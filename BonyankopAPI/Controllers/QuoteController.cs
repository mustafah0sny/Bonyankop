using BonyankopAPI.DTOs;
using BonyankopAPI.Interfaces;
using BonyankopAPI.Models;
using BonyankopAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace BonyankopAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class QuoteController : ControllerBase
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly IProviderProfileRepository _providerProfileRepository;
    private readonly IUserRepository _userRepository;

    public QuoteController(
        IQuoteRepository quoteRepository,
        IServiceRequestRepository serviceRequestRepository,
        IProviderProfileRepository providerProfileRepository,
        IUserRepository userRepository)
    {
        _quoteRepository = quoteRepository;
        _serviceRequestRepository = serviceRequestRepository;
        _providerProfileRepository = providerProfileRepository;
        _userRepository = userRepository;
    }

    [HttpPost]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> CreateQuote([FromBody] CreateQuoteDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Get provider profile
        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null)
        {
            return BadRequest(new { message = "Provider profile not found. Please create a provider profile first." });
        }

        // Validate service request
        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(dto.RequestId);
        if (serviceRequest == null)
        {
            return NotFound(new { message = "Service request not found" });
        }

        if (serviceRequest.Status != RequestStatus.OPEN && serviceRequest.Status != RequestStatus.QUOTES_RECEIVED)
        {
            return BadRequest(new { message = "Service request is not accepting quotes" });
        }

        // Check if provider already submitted a quote
        var existingQuote = await _quoteRepository.GetByRequestAndProviderAsync(dto.RequestId, providerProfile.ProviderId);
        if (existingQuote != null)
        {
            return BadRequest(new { message = "You have already submitted a quote for this request" });
        }

        var costBreakdown = new CostBreakdown
        {
            LaborCost = dto.CostBreakdown.LaborCost,
            MaterialsCost = dto.CostBreakdown.MaterialsCost,
            EquipmentCost = dto.CostBreakdown.EquipmentCost,
            OtherCosts = dto.CostBreakdown.OtherCosts,
            TaxAmount = dto.CostBreakdown.TaxAmount,
            TotalAmount = dto.CostBreakdown.TotalAmount
        };

        var quote = new Quote
        {
            RequestId = dto.RequestId,
            ProviderId = providerProfile.ProviderId,
            EstimatedCost = dto.EstimatedCost,
            CostBreakdownJson = JsonSerializer.Serialize(costBreakdown),
            EstimatedDurationDays = dto.EstimatedDurationDays,
            TechnicalAssessment = dto.TechnicalAssessment,
            ProposedSolution = dto.ProposedSolution,
            MaterialsIncluded = dto.MaterialsIncluded,
            WarrantyPeriodMonths = dto.WarrantyPeriodMonths,
            TermsAndConditions = dto.TermsAndConditions,
            ValidityPeriodDays = dto.ValidityPeriodDays,
            Attachments = dto.Attachments,
            ExpiresAt = DateTime.UtcNow.AddDays(dto.ValidityPeriodDays)
        };

        await _quoteRepository.AddAsync(quote);
        await _quoteRepository.SaveChangesAsync();

        // Update service request quotes count
        await _serviceRequestRepository.UpdateQuotesCountAsync(dto.RequestId);

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        var response = new QuoteResponseDto
        {
            QuoteId = quote.QuoteId,
            RequestId = quote.RequestId,
            ProviderId = quote.ProviderId,
            ProviderName = user?.FullName ?? "Unknown",
            EstimatedCost = quote.EstimatedCost,
            CostBreakdown = dto.CostBreakdown,
            EstimatedDurationDays = quote.EstimatedDurationDays,
            TechnicalAssessment = quote.TechnicalAssessment,
            ProposedSolution = quote.ProposedSolution,
            MaterialsIncluded = quote.MaterialsIncluded,
            WarrantyPeriodMonths = quote.WarrantyPeriodMonths,
            TermsAndConditions = quote.TermsAndConditions,
            ValidityPeriodDays = quote.ValidityPeriodDays,
            Attachments = quote.Attachments,
            Status = quote.Status,
            ExpiresAt = quote.ExpiresAt,
            SubmittedAt = quote.SubmittedAt,
            UpdatedAt = quote.UpdatedAt,
            IsExpired = quote.IsExpired()
        };

        return CreatedAtAction(nameof(GetQuote), new { id = quote.QuoteId }, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuote(Guid id)
    {
        var quote = await _quoteRepository.GetByIdAsync(id);
        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        var providerProfile = await _providerProfileRepository.GetByIdAsync(quote.ProviderId);
        var user = providerProfile != null ? await _userRepository.GetByIdAsync(providerProfile.UserId) : null;

        var costBreakdown = JsonSerializer.Deserialize<CostBreakdown>(quote.CostBreakdownJson);
        var response = new QuoteResponseDto
        {
            QuoteId = quote.QuoteId,
            RequestId = quote.RequestId,
            ProviderId = quote.ProviderId,
            ProviderName = user?.FullName ?? "Unknown",
            EstimatedCost = quote.EstimatedCost,
            CostBreakdown = new CostBreakdownDto
            {
                LaborCost = costBreakdown!.LaborCost,
                MaterialsCost = costBreakdown.MaterialsCost,
                EquipmentCost = costBreakdown.EquipmentCost,
                OtherCosts = costBreakdown.OtherCosts,
                TaxAmount = costBreakdown.TaxAmount,
                TotalAmount = costBreakdown.TotalAmount
            },
            EstimatedDurationDays = quote.EstimatedDurationDays,
            TechnicalAssessment = quote.TechnicalAssessment,
            ProposedSolution = quote.ProposedSolution,
            MaterialsIncluded = quote.MaterialsIncluded,
            WarrantyPeriodMonths = quote.WarrantyPeriodMonths,
            TermsAndConditions = quote.TermsAndConditions,
            ValidityPeriodDays = quote.ValidityPeriodDays,
            Attachments = quote.Attachments,
            Status = quote.Status,
            ExpiresAt = quote.ExpiresAt,
            SubmittedAt = quote.SubmittedAt,
            UpdatedAt = quote.UpdatedAt,
            IsExpired = quote.IsExpired()
        };

        return Ok(response);
    }

    [HttpGet("request/{requestId}")]
    public async Task<IActionResult> GetQuotesByRequest(Guid requestId)
    {
        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(requestId);
        if (serviceRequest == null)
        {
            return NotFound(new { message = "Service request not found" });
        }

        var quotes = await _quoteRepository.GetByRequestIdAsync(requestId);
        
        var response = new List<QuoteResponseDto>();
        foreach (var quote in quotes)
        {
            var providerProfile = await _providerProfileRepository.GetByIdAsync(quote.ProviderId);
            var user = providerProfile != null ? await _userRepository.GetByIdAsync(providerProfile.UserId) : null;
            var costBreakdown = JsonSerializer.Deserialize<CostBreakdown>(quote.CostBreakdownJson);

            response.Add(new QuoteResponseDto
            {
                QuoteId = quote.QuoteId,
                RequestId = quote.RequestId,
                ProviderId = quote.ProviderId,
                ProviderName = user?.FullName ?? "Unknown",
                EstimatedCost = quote.EstimatedCost,
                CostBreakdown = new CostBreakdownDto
                {
                    LaborCost = costBreakdown!.LaborCost,
                    MaterialsCost = costBreakdown.MaterialsCost,
                    EquipmentCost = costBreakdown.EquipmentCost,
                    OtherCosts = costBreakdown.OtherCosts,
                    TaxAmount = costBreakdown.TaxAmount,
                    TotalAmount = costBreakdown.TotalAmount
                },
                EstimatedDurationDays = quote.EstimatedDurationDays,
                TechnicalAssessment = quote.TechnicalAssessment,
                ProposedSolution = quote.ProposedSolution,
                MaterialsIncluded = quote.MaterialsIncluded,
                WarrantyPeriodMonths = quote.WarrantyPeriodMonths,
                TermsAndConditions = quote.TermsAndConditions,
                ValidityPeriodDays = quote.ValidityPeriodDays,
                Attachments = quote.Attachments,
                Status = quote.Status,
                ExpiresAt = quote.ExpiresAt,
                SubmittedAt = quote.SubmittedAt,
                UpdatedAt = quote.UpdatedAt,
                IsExpired = quote.IsExpired()
            });
        }

        return Ok(response);
    }

    [HttpGet("my-quotes")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> GetMyQuotes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null)
        {
            return BadRequest(new { message = "Provider profile not found" });
        }

        var quotes = await _quoteRepository.GetByProviderIdAsync(providerProfile.ProviderId);
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        var response = quotes.Select(quote =>
        {
            var costBreakdown = JsonSerializer.Deserialize<CostBreakdown>(quote.CostBreakdownJson);
            return new QuoteResponseDto
            {
                QuoteId = quote.QuoteId,
                RequestId = quote.RequestId,
                ProviderId = quote.ProviderId,
                ProviderName = user?.FullName ?? "Unknown",
                EstimatedCost = quote.EstimatedCost,
                CostBreakdown = new CostBreakdownDto
                {
                    LaborCost = costBreakdown!.LaborCost,
                    MaterialsCost = costBreakdown.MaterialsCost,
                    EquipmentCost = costBreakdown.EquipmentCost,
                    OtherCosts = costBreakdown.OtherCosts,
                    TaxAmount = costBreakdown.TaxAmount,
                    TotalAmount = costBreakdown.TotalAmount
                },
                EstimatedDurationDays = quote.EstimatedDurationDays,
                TechnicalAssessment = quote.TechnicalAssessment,
                ProposedSolution = quote.ProposedSolution,
                MaterialsIncluded = quote.MaterialsIncluded,
                WarrantyPeriodMonths = quote.WarrantyPeriodMonths,
                TermsAndConditions = quote.TermsAndConditions,
                ValidityPeriodDays = quote.ValidityPeriodDays,
                Attachments = quote.Attachments,
                Status = quote.Status,
                ExpiresAt = quote.ExpiresAt,
                SubmittedAt = quote.SubmittedAt,
                UpdatedAt = quote.UpdatedAt,
                IsExpired = quote.IsExpired()
            };
        });

        return Ok(response);
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> UpdateQuote(Guid id, [FromBody] UpdateQuoteDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null)
        {
            return BadRequest(new { message = "Provider profile not found" });
        }

        var quote = await _quoteRepository.GetByIdAsync(id);
        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        if (quote.ProviderId != providerProfile.ProviderId)
        {
            return Forbid();
        }

        if (quote.Status != QuoteStatus.PENDING)
        {
            return BadRequest(new { message = "Cannot update quote in current status" });
        }

        if (quote.IsExpired())
        {
            return BadRequest(new { message = "Cannot update expired quote" });
        }

        if (dto.EstimatedCost.HasValue) quote.EstimatedCost = dto.EstimatedCost.Value;
        
        if (dto.CostBreakdown != null)
        {
            var costBreakdown = new CostBreakdown
            {
                LaborCost = dto.CostBreakdown.LaborCost,
                MaterialsCost = dto.CostBreakdown.MaterialsCost,
                EquipmentCost = dto.CostBreakdown.EquipmentCost,
                OtherCosts = dto.CostBreakdown.OtherCosts,
                TaxAmount = dto.CostBreakdown.TaxAmount,
                TotalAmount = dto.CostBreakdown.TotalAmount
            };
            quote.CostBreakdownJson = JsonSerializer.Serialize(costBreakdown);
        }

        if (dto.EstimatedDurationDays.HasValue) quote.EstimatedDurationDays = dto.EstimatedDurationDays.Value;
        if (dto.TechnicalAssessment != null) quote.TechnicalAssessment = dto.TechnicalAssessment;
        if (dto.ProposedSolution != null) quote.ProposedSolution = dto.ProposedSolution;
        if (dto.MaterialsIncluded.HasValue) quote.MaterialsIncluded = dto.MaterialsIncluded.Value;
        if (dto.WarrantyPeriodMonths.HasValue) quote.WarrantyPeriodMonths = dto.WarrantyPeriodMonths.Value;
        if (dto.TermsAndConditions != null) quote.TermsAndConditions = dto.TermsAndConditions;
        if (dto.ValidityPeriodDays.HasValue)
        {
            quote.ValidityPeriodDays = dto.ValidityPeriodDays.Value;
            quote.ExpiresAt = DateTime.UtcNow.AddDays(dto.ValidityPeriodDays.Value);
        }
        if (dto.Attachments != null) quote.Attachments = dto.Attachments;

        quote.UpdatedAt = DateTime.UtcNow;

        _quoteRepository.Update(quote);
        await _quoteRepository.SaveChangesAsync();

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        var currentCostBreakdown = JsonSerializer.Deserialize<CostBreakdown>(quote.CostBreakdownJson);
        
        var response = new QuoteResponseDto
        {
            QuoteId = quote.QuoteId,
            RequestId = quote.RequestId,
            ProviderId = quote.ProviderId,
            ProviderName = user?.FullName ?? "Unknown",
            EstimatedCost = quote.EstimatedCost,
            CostBreakdown = new CostBreakdownDto
            {
                LaborCost = currentCostBreakdown!.LaborCost,
                MaterialsCost = currentCostBreakdown.MaterialsCost,
                EquipmentCost = currentCostBreakdown.EquipmentCost,
                OtherCosts = currentCostBreakdown.OtherCosts,
                TaxAmount = currentCostBreakdown.TaxAmount,
                TotalAmount = currentCostBreakdown.TotalAmount
            },
            EstimatedDurationDays = quote.EstimatedDurationDays,
            TechnicalAssessment = quote.TechnicalAssessment,
            ProposedSolution = quote.ProposedSolution,
            MaterialsIncluded = quote.MaterialsIncluded,
            WarrantyPeriodMonths = quote.WarrantyPeriodMonths,
            TermsAndConditions = quote.TermsAndConditions,
            ValidityPeriodDays = quote.ValidityPeriodDays,
            Attachments = quote.Attachments,
            Status = quote.Status,
            ExpiresAt = quote.ExpiresAt,
            SubmittedAt = quote.SubmittedAt,
            UpdatedAt = quote.UpdatedAt,
            IsExpired = quote.IsExpired()
        };

        return Ok(response);
    }

    [HttpDelete("{id}/withdraw")]
    //[Authorize(Roles = "COMPANY,ENGINEER")]
    public async Task<IActionResult> WithdrawQuote(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var providerProfile = await _providerProfileRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (providerProfile == null)
        {
            return BadRequest(new { message = "Provider profile not found" });
        }

        var quote = await _quoteRepository.GetByIdAsync(id);
        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        if (quote.ProviderId != providerProfile.ProviderId)
        {
            return Forbid();
        }

        if (quote.Status != QuoteStatus.PENDING)
        {
            return BadRequest(new { message = "Can only withdraw pending quotes" });
        }

        quote.Status = QuoteStatus.WITHDRAWN;
        quote.UpdatedAt = DateTime.UtcNow;

        _quoteRepository.Update(quote);
        await _quoteRepository.SaveChangesAsync();

        // Update service request quotes count
        await _serviceRequestRepository.UpdateQuotesCountAsync(quote.RequestId);

        return Ok(new { message = "Quote withdrawn successfully" });
    }

    [HttpPost("{id}/accept")]
    //[Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> AcceptQuote(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var quote = await _quoteRepository.GetByIdAsync(id);
        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(quote.RequestId);
        if (serviceRequest == null || serviceRequest.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        if (quote.Status != QuoteStatus.PENDING)
        {
            return BadRequest(new { message = "Quote is not in pending status" });
        }

        if (quote.IsExpired())
        {
            return BadRequest(new { message = "Quote has expired" });
        }

        quote.Status = QuoteStatus.ACCEPTED;
        quote.UpdatedAt = DateTime.UtcNow;

        _quoteRepository.Update(quote);
        await _quoteRepository.SaveChangesAsync();

        // Update service request status
        serviceRequest.Status = RequestStatus.PROVIDER_SELECTED;
        serviceRequest.UpdatedAt = DateTime.UtcNow;
        _serviceRequestRepository.Update(serviceRequest);
        await _serviceRequestRepository.SaveChangesAsync();

        return Ok(new { message = "Quote accepted successfully" });
    }

    [HttpPost("{id}/reject")]
    //[Authorize(Roles = "CITIZEN")]
    public async Task<IActionResult> RejectQuote(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var quote = await _quoteRepository.GetByIdAsync(id);
        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        var serviceRequest = await _serviceRequestRepository.GetByIdAsync(quote.RequestId);
        if (serviceRequest == null || serviceRequest.CitizenId != Guid.Parse(userId))
        {
            return Forbid();
        }

        if (quote.Status != QuoteStatus.PENDING)
        {
            return BadRequest(new { message = "Quote is not in pending status" });
        }

        quote.Status = QuoteStatus.REJECTED;
        quote.UpdatedAt = DateTime.UtcNow;

        _quoteRepository.Update(quote);
        await _quoteRepository.SaveChangesAsync();

        return Ok(new { message = "Quote rejected successfully" });
    }
}
