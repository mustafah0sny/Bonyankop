# Database Design Wiki - Smart Home Maintenance Platform

## 📋 Table of Contents
1. [Database Overview](#database-overview)
2. [Database Technology Selection](#database-technology-selection)
3. [Database Architecture](#database-architecture)
4. [Entity Relationship Diagram (ERD)](#entity-relationship-diagram-erd)
5. [Detailed Table Specifications](#detailed-table-specifications)
6. [Relationships and Foreign Keys](#relationships-and-foreign-keys)
7. [Indexing Strategy](#indexing-strategy)
8. [Data Integrity and Constraints](#data-integrity-and-constraints)
9. [Database Security](#database-security)
10. [Backup and Recovery](#backup-and-recovery)
11. [Performance Optimization](#performance-optimization)
12. [Scalability Considerations](#scalability-considerations)
13. [Data Migration Strategy](#data-migration-strategy)
14. [Sample Data Scenarios](#sample-data-scenarios)

---

## 🗄️ Database Overview

### Purpose
The database serves as the central data repository for the Smart Home Maintenance & Public Infrastructure Platform, storing all information related to users, diagnostics, service requests, projects, public reports, and system operations.

### Database Scope
The database manages:
- **User Management:** All user accounts across different roles
- **Diagnostic Records:** AI-powered home problem diagnostics
- **Service Marketplace:** Requests, quotes, and project management
- **Public Reporting:** Infrastructure issue tracking
- **Communication:** Chat messages and notifications
- **Analytics:** Ratings, reviews, and system metrics
- **Audit Trail:** Complete system activity logging

### Key Requirements
- **Data Integrity:** Maintain consistent and accurate data
- **Performance:** Fast query execution for real-time operations
- **Scalability:** Support growing user base and data volume
- **Security:** Protect sensitive user information
- **Reliability:** High availability and data durability
- **Compliance:** Meet data protection regulations

---

## 💾 Database Technology Selection

### Primary Database: PostgreSQL

#### Why PostgreSQL?

**1. Relational Data Model**
- Strong support for complex relationships
- ACID compliance ensures data consistency
- Foreign key constraints maintain referential integrity
- Transaction support for multi-step operations

**2. Advanced Features**
- **JSONB Data Type:** Store flexible, semi-structured data (services offered, image arrays)
- **Full-Text Search:** Search through descriptions and reviews
- **PostGIS Extension:** Geospatial queries for location-based features
- **Array Data Types:** Store lists of values efficiently
- **Triggers and Stored Procedures:** Automate database operations

**3. Performance**
- Excellent query optimization
- Support for complex joins
- Efficient indexing (B-tree, Hash, GiST, GIN)
- Materialized views for complex aggregations
- Parallel query execution

**4. Scalability**
- Vertical scaling (more powerful hardware)
- Horizontal scaling (read replicas)
- Partitioning support for large tables
- Connection pooling

**5. Open Source**
- No licensing costs
- Active community support
- Regular updates and security patches
- Extensive documentation

**6. Ecosystem Compatibility**
- Excellent Node.js support (node-postgres, Sequelize, Prisma)
- Works well with all major cloud providers
- Compatible with popular ORMs
- Wide tool support (pgAdmin, DBeaver, DataGrip)

### Supporting Databases

#### Redis (In-Memory Cache)

**Purpose:** High-speed data access and temporary storage

**Use Cases:**
- **Session Storage:** User authentication tokens
- **Cache Layer:** Frequently accessed data (user profiles, provider ratings)
- **Real-Time Features:** Chat message queuing
- **Rate Limiting:** API request counters
- **Temporary Data:** OTP codes, verification tokens

**Benefits:**
- Extremely fast (sub-millisecond response times)
- Reduces database load
- Supports various data structures (strings, hashes, lists, sets)
- Pub/Sub for real-time notifications

#### Cloud Object Storage (AWS S3 / Google Cloud Storage)

**Purpose:** Store large binary files

**Stored Content:**
- Uploaded diagnostic images
- Project photos (before/after)
- Public report images
- User profile pictures
- Technical reports and documents
- Provider portfolio images

**Benefits:**
- Unlimited scalability
- High durability (99.999999999%)
- Cost-effective for large files
- CDN integration for fast delivery
- Automatic backup and versioning

---

## 🏗️ Database Architecture

### Three-Layer Architecture

#### Layer 1: Application Layer
- **ORM/Query Builder:** Sequelize or Prisma
- **Connection Pooling:** Manage database connections efficiently
- **Query Optimization:** Prepare and optimize queries
- **Transaction Management:** Handle multi-step operations

#### Layer 2: Database Server Layer
- **PostgreSQL Server:** Main database engine
- **Query Processor:** Parse and execute SQL queries
- **Storage Engine:** Manage data storage and retrieval
- **Cache Manager:** Buffer pool for frequently accessed data

#### Layer 3: Storage Layer
- **Data Files:** Actual table data storage
- **Index Files:** B-tree and other index structures
- **Write-Ahead Log (WAL):** Transaction logging for recovery
- **Configuration Files:** Database settings

### Database Instances

#### Production Database
- **Primary Instance:** Handle all write operations
- **Read Replicas:** Distribute read operations (optional)
- **Backup Instance:** Continuous backup for disaster recovery

#### Development Database
- **Local Instance:** Developer machines
- **Staging Instance:** Pre-production testing
- **Test Instance:** Automated testing

### Connection Architecture

**Connection Pooling Strategy:**
- **Minimum Connections:** 10 (always available)
- **Maximum Connections:** 100 (prevent overload)
- **Idle Timeout:** 30 seconds (release unused connections)
- **Connection Lifetime:** 30 minutes (prevent stale connections)

**Connection Distribution:**
- **Web Application:** 60% of pool
- **Background Jobs:** 20% of pool
- **Admin Operations:** 10% of pool
- **Monitoring/Analytics:** 10% of pool

---

## 📊 Entity Relationship Diagram (ERD)

### High-Level Entity Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     CORE ENTITIES                            │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────┐         ┌──────────────────┐                 │
│  │  USERS   │────────▶│ PROVIDER_PROFILES│                 │
│  └────┬─────┘         └─────────┬────────┘                 │
│       │                          │                           │
│       │                          │                           │
│       ├──────────────────────────┼───────────────┐          │
│       │                          │               │          │
│       ▼                          ▼               ▼          │
│  ┌────────────┐         ┌──────────────┐  ┌─────────────┐ │
│  │ DIAGNOSTICS│         │   QUOTES     │  │  PORTFOLIO  │ │
│  └─────┬──────┘         └──────┬───────┘  │   ITEMS     │ │
│        │                       │           └─────────────┘ │
│        │                       │                           │
│        ▼                       │                           │
│  ┌──────────────┐              │                           │
│  │   SERVICE    │◀─────────────┘                           │
│  │   REQUESTS   │                                          │
│  └──────┬───────┘                                          │
│         │                                                   │
│         ▼                                                   │
│  ┌──────────────┐                                          │
│  │   PROJECTS   │                                          │
│  └──────┬───────┘                                          │
│         │                                                   │
│         ├─────────────┬──────────────┐                     │
│         ▼             ▼              ▼                     │
│  ┌────────────┐ ┌──────────┐  ┌─────────┐                │
│  │   CHAT     │ │ RATINGS  │  │ PROJECT │                │
│  │  MESSAGES  │ │          │  │  LOGS   │                │
│  └────────────┘ └──────────┘  └─────────┘                │
│                                                              │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                  PUBLIC REPORTING ENTITIES                   │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────┐                                               │
│  │  USERS   │                                               │
│  └────┬─────┘                                               │
│       │                                                      │
│       ▼                                                      │
│  ┌──────────────┐                                           │
│  │   PUBLIC     │                                           │
│  │   REPORTS    │                                           │
│  └──────┬───────┘                                           │
│         │                                                    │
│         ▼                                                    │
│  ┌──────────────┐                                           │
│  │   REPORT     │                                           │
│  │   UPDATES    │                                           │
│  └──────────────┘                                           │
│                                                              │
│  ┌──────────────┐                                           │
│  │ GOVERNMENT   │                                           │
│  │ANNOUNCEMENTS │                                           │
│  └──────────────┘                                           │
│                                                              │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                    SYSTEM ENTITIES                           │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────┐         ┌──────────────┐                 │
│  │NOTIFICATIONS │         │  AUDIT_LOGS  │                 │
│  └──────────────┘         └──────────────┘                 │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Relationship Types

**One-to-One (1:1):**
- User ↔ Provider Profile (one user can have one provider profile)
- Service Request ↔ Project (one accepted request becomes one project)
- Quote ↔ Project (one accepted quote becomes one project)
- Project ↔ Rating (one project receives one rating)

**One-to-Many (1:N):**
- User → Diagnostics (one user can have many diagnostics)
- User → Service Requests (one citizen can create many requests)
- User → Public Reports (one citizen can submit many reports)
- Provider Profile → Quotes (one provider can submit many quotes)
- Service Request → Quotes (one request can receive many quotes)
- Project → Chat Messages (one project can have many messages)
- Public Report → Report Updates (one report can have many status updates)

**Many-to-One (N:1):**
- All child entities back to their parent entities

---

## 📋 Detailed Table Specifications

### 1. USERS Table

**Purpose:** Central table storing all user accounts across different roles

**Table Name:** `users`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| user_id | UUID | PRIMARY KEY | Unique identifier for each user |
| email | VARCHAR(255) | UNIQUE, NOT NULL | User's email address (login credential) |
| password_hash | VARCHAR(255) | NOT NULL | Bcrypt hashed password |
| role | VARCHAR(50) | NOT NULL | User role: 'citizen', 'engineer', 'company', 'government', 'admin' |
| full_name | VARCHAR(255) | NOT NULL | User's complete name |
| phone_number | VARCHAR(20) | NULL | Contact phone number |
| profile_picture_url | TEXT | NULL | URL to profile image in cloud storage |
| is_verified | BOOLEAN | DEFAULT FALSE | Email verification status |
| is_active | BOOLEAN | DEFAULT TRUE | Account active status (for soft delete) |
| last_login_at | TIMESTAMP | NULL | Last successful login timestamp |
| created_at | TIMESTAMP | DEFAULT NOW() | Account creation timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last profile update timestamp |

**Indexes:**
- PRIMARY KEY on `user_id`
- UNIQUE INDEX on `email`
- INDEX on `role` (for role-based queries)
- INDEX on `is_active` (for filtering active users)
- INDEX on `created_at` (for sorting by registration date)

**Business Rules:**
- Email must be unique across all users
- Password must be hashed using bcrypt (minimum 10 rounds)
- Role cannot be changed after registration (security)
- Deleted users are marked `is_active = FALSE` (soft delete)
- Phone number format should be validated at application level

**Sample Data:**
```
user_id: 550e8400-e29b-41d4-a716-446655440000
email: ahmed.citizen@example.com
role: citizen
full_name: Ahmed Mohammed
phone_number: +966501234567
is_verified: TRUE
is_active: TRUE
```

---

### 2. PROVIDER_PROFILES Table

**Purpose:** Extended information for service providers (companies and engineers)

**Table Name:** `provider_profiles`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| provider_id | UUID | PRIMARY KEY | Unique identifier for provider profile |
| user_id | UUID | FOREIGN KEY → users(user_id), UNIQUE | Link to user account |
| provider_type | VARCHAR(50) | NOT NULL | 'company' or 'engineer' |
| business_name | VARCHAR(255) | NOT NULL | Company name or professional name |
| description | TEXT | NULL | About the provider (bio, experience) |
| services_offered | JSONB | NULL | Array of service types: ["plumbing", "electrical", "hvac"] |
| certifications | JSONB | NULL | Array of certificate objects with URLs |
| coverage_areas | JSONB | NULL | Array of geographic areas served |
| license_number | VARCHAR(100) | NULL | Professional license or registration number |
| years_of_experience | INTEGER | NULL | Years in business |
| average_rating | DECIMAL(3,2) | DEFAULT 0.00 | Calculated average rating (0.00 to 5.00) |
| total_projects | INTEGER | DEFAULT 0 | Count of completed projects |
| total_ratings | INTEGER | DEFAULT 0 | Count of ratings received |
| completion_rate | DECIMAL(5,2) | DEFAULT 0.00 | Percentage of completed projects (0.00 to 100.00) |
| response_time_hours | DECIMAL(5,2) | NULL | Average time to respond to requests (in hours) |
| is_verified | BOOLEAN | DEFAULT FALSE | Admin verification status |
| is_featured | BOOLEAN | DEFAULT FALSE | Featured provider flag |
| created_at | TIMESTAMP | DEFAULT NOW() | Profile creation timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last profile update timestamp |

**Indexes:**
- PRIMARY KEY on `provider_id`
- UNIQUE INDEX on `user_id`
- INDEX on `provider_type`
- INDEX on `average_rating DESC` (for sorting by rating)
- INDEX on `is_verified`
- GIN INDEX on `services_offered` (for JSONB queries)
- GIN INDEX on `coverage_areas` (for location-based searches)

**Business Rules:**
- One user can have only one provider profile
- Average rating is calculated automatically from ratings table
- Total projects updated via trigger when project status changes
- Completion rate = (completed_projects / total_projects) * 100
- Services offered must be from predefined list
- Verification required before appearing in search results

**JSONB Structure Examples:**

**services_offered:**
```json
[
  "plumbing",
  "electrical",
  "hvac",
  "carpentry",
  "painting",
  "roofing"
]
```

**certifications:**
```json
[
  {
    "name": "Licensed Electrician",
    "issuer": "Ministry of Labor",
    "issue_date": "2020-01-15",
    "expiry_date": "2025-01-15",
    "certificate_url": "https://storage.example.com/certs/cert123.pdf"
  }
]
```

**coverage_areas:**
```json
[
  {
    "city": "Riyadh",
    "districts": ["Al Olaya", "Al Malqa", "Al Nakheel"]
  },
  {
    "city": "Jeddah",
    "districts": ["Al Hamra", "Al Rawdah"]
  }
]
```

---

### 3. PORTFOLIO_ITEMS Table

**Purpose:** Showcase provider's previous work with images

**Table Name:** `portfolio_items`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| portfolio_id | UUID | PRIMARY KEY | Unique identifier for portfolio item |
| provider_id | UUID | FOREIGN KEY → provider_profiles(provider_id) | Owner of portfolio item |
| title | VARCHAR(255) | NOT NULL | Project title |
| description | TEXT | NULL | Detailed description of the work |
| project_type | VARCHAR(100) | NULL | Type of work (plumbing, electrical, etc.) |
| images | JSONB | NULL | Array of image URLs |
| project_date | DATE | NULL | When the project was completed |
| location | VARCHAR(255) | NULL | Project location (optional) |
| display_order | INTEGER | DEFAULT 0 | Order for displaying items |
| is_featured | BOOLEAN | DEFAULT FALSE | Highlight this item |
| created_at | TIMESTAMP | DEFAULT NOW() | Item creation timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `portfolio_id`
- INDEX on `provider_id`
- INDEX on `project_type`
- INDEX on `display_order`
- INDEX on `is_featured`

**Business Rules:**
- Maximum 20 portfolio items per provider
- At least one image required
- Images stored in cloud storage, only URLs in database
- Featured items appear first in provider profile

**JSONB Structure - images:**
```json
[
  {
    "url": "https://storage.example.com/portfolio/img1.jpg",
    "caption": "Before renovation",
    "order": 1
  },
  {
    "url": "https://storage.example.com/portfolio/img2.jpg",
    "caption": "After renovation",
    "order": 2
  }
]
```

---

### 4. DIAGNOSTICS Table

**Purpose:** Store AI diagnostic results for home problems

**Table Name:** `diagnostics`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| diagnostic_id | UUID | PRIMARY KEY | Unique identifier for diagnostic |
| citizen_id | UUID | FOREIGN KEY → users(user_id) | User who uploaded the image |
| image_url | TEXT | NOT NULL | URL to uploaded image in cloud storage |
| image_metadata | JSONB | NULL | Image details (size, format, dimensions) |
| risk_level | VARCHAR(50) | NOT NULL | 'low', 'medium', 'high' |
| problem_category | VARCHAR(100) | NOT NULL | 'plumbing', 'electrical', 'structural', 'hvac', etc. |
| problem_subcategory | VARCHAR(100) | NULL | More specific classification |
| probable_cause | TEXT | NOT NULL | AI-identified cause of the problem |
| risk_prediction | TEXT | NOT NULL | What happens if not fixed |
| recommended_action | TEXT | NOT NULL | Suggested next steps |
| ai_confidence_score | DECIMAL(5,2) | NOT NULL | Model confidence (0.00 to 100.00) |
| ai_model_version | VARCHAR(50) | NULL | Version of AI model used |
| processing_time_ms | INTEGER | NULL | Time taken for analysis (milliseconds) |
| is_diy_possible | BOOLEAN | DEFAULT FALSE | Can be fixed by homeowner |
| estimated_cost_range | VARCHAR(100) | NULL | Approximate repair cost range |
| urgency_level | VARCHAR(50) | NULL | 'immediate', 'soon', 'can_wait' |
| created_at | TIMESTAMP | DEFAULT NOW() | Diagnostic timestamp |

**Indexes:**
- PRIMARY KEY on `diagnostic_id`
- INDEX on `citizen_id`
- INDEX on `risk_level`
- INDEX on `problem_category`
- INDEX on `created_at DESC` (for recent diagnostics)
- INDEX on `ai_confidence_score` (for quality filtering)

**Business Rules:**
- Each diagnostic is independent (can run multiple for same problem)
- Images automatically deleted after 90 days (GDPR compliance)
- Low confidence results (<70%) show warning to user
- DIY solutions only shown for low-risk problems
- Diagnostic results stored permanently for home history

**JSONB Structure - image_metadata:**
```json
{
  "original_filename": "kitchen_leak.jpg",
  "file_size_bytes": 2458624,
  "dimensions": {
    "width": 1920,
    "height": 1080
  },
  "format": "JPEG",
  "upload_timestamp": "2024-11-16T10:30:00Z"
}
```

---

### 5. SERVICE_REQUESTS Table

**Purpose:** Track citizen requests for professional services

**Table Name:** `service_requests`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| request_id | UUID | PRIMARY KEY | Unique identifier for service request |
| diagnostic_id | UUID | FOREIGN KEY → diagnostics(diagnostic_id), NULL | Related diagnostic (optional) |
| citizen_id | UUID | FOREIGN KEY → users(user_id) | User requesting service |
| problem_title | VARCHAR(255) | NOT NULL | Brief description of problem |
| problem_description | TEXT | NOT NULL | Detailed description |
| problem_category | VARCHAR(100) | NOT NULL | Type of problem |
| additional_images | JSONB | NULL | Extra images beyond diagnostic |
| preferred_provider_type | VARCHAR(50) | NULL | 'company', 'engineer', 'any' |
| preferred_service_date | DATE | NULL | When service is needed |
| property_type | VARCHAR(50) | NULL | 'apartment', 'villa', 'commercial' |
| property_address | TEXT | NULL | Service location |
| contact_phone | VARCHAR(20) | NULL | Contact number for this request |
| status | VARCHAR(50) | DEFAULT 'open' | 'open', 'quotes_received', 'provider_selected', 'cancelled' |
| selected_quote_id | UUID | FOREIGN KEY → quotes(quote_id), NULL | Accepted quote |
| quotes_count | INTEGER | DEFAULT 0 | Number of quotes received |
| views_count | INTEGER | DEFAULT 0 | How many providers viewed this |
| expires_at | TIMESTAMP | NULL | Request expiration (auto-close) |
| created_at | TIMESTAMP | DEFAULT NOW() | Request creation timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `request_id`
- INDEX on `citizen_id`
- INDEX on `diagnostic_id`
- INDEX on `status`
- INDEX on `problem_category`
- INDEX on `created_at DESC`
- INDEX on `expires_at` (for cleanup jobs)

**Business Rules:**
- Request can be created with or without diagnostic
- Automatically expires after 30 days if no provider selected
- Status changes: open → quotes_received → provider_selected
- Cannot be deleted, only cancelled
- Quotes count updated via trigger
- Citizen can cancel request anytime before provider selection

**Status Workflow:**
```
open → quotes_received → provider_selected → [becomes project]
  ↓
cancelled (terminal state)
```

---

### 6. QUOTES Table

**Purpose:** Store provider quotes for service requests

**Table Name:** `quotes`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| quote_id | UUID | PRIMARY KEY | Unique identifier for quote |
| request_id | UUID | FOREIGN KEY → service_requests(request_id) | Related service request |
| provider_id | UUID | FOREIGN KEY → provider_profiles(provider_id) | Provider submitting quote |
| estimated_cost | DECIMAL(10,2) | NOT NULL | Total estimated cost |
| cost_breakdown | JSONB | NULL | Itemized cost details |
| estimated_duration_days | INTEGER | NULL | Expected completion time |
| technical_assessment | TEXT | NULL | Provider's professional opinion |
| proposed_solution | TEXT | NULL | How provider will fix the problem |
| materials_included | BOOLEAN | DEFAULT TRUE | Whether materials are in the price |
| warranty_period_months | INTEGER | NULL | Warranty offered (in months) |
| terms_and_conditions | TEXT | NULL | Service terms |
| validity_period_days | INTEGER | DEFAULT 7 | How long quote is valid |
| attachments | JSONB | NULL | Supporting documents or diagrams |
| status | VARCHAR(50) | DEFAULT 'pending' | 'pending', 'accepted', 'rejected', 'expired', 'withdrawn' |
| rejection_reason | TEXT | NULL | Why citizen rejected (optional) |
| submitted_at | TIMESTAMP | DEFAULT NOW() | Quote submission timestamp |
| expires_at | TIMESTAMP | NOT NULL | Quote expiration timestamp |
| accepted_at | TIMESTAMP | NULL | When quote was accepted |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `quote_id`
- INDEX on `request_id`
- INDEX on `provider_id`
- INDEX on `status`
- INDEX on `submitted_at DESC`
- INDEX on `expires_at`

**Business Rules:**
- One provider can submit only one quote per request
- Quote automatically expires after validity period
- Cannot modify quote after submission (must withdraw and resubmit)
- Accepted quote creates a project
- Provider can withdraw quote before acceptance
- Citizen can reject with optional reason

**JSONB Structure - cost_breakdown:**
```json
{
  "labor": {
    "description": "Labor cost",
    "amount": 500.00,
    "hours": 8
  },
  "materials": [
    {
      "item": "Copper pipes",
      "quantity": 10,
      "unit_price": 25.00,
      "total": 250.00
    },
    {
      "item": "Fittings",
      "quantity": 15,
      "unit_price": 5.00,
      "total": 75.00
    }
  ],
  "equipment": {
    "description": "Equipment rental",
    "amount": 100.00
  },
  "tax": {
    "rate": 15,
    "amount": 138.75
  },
  "total": 1063.75
}
```

---

### 7. PROJECTS Table

**Purpose:** Track ongoing and completed maintenance projects

**Table Name:** `projects`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| project_id | UUID | PRIMARY KEY | Unique identifier for project |
| request_id | UUID | FOREIGN KEY → service_requests(request_id), UNIQUE | Original service request |
| quote_id | UUID | FOREIGN KEY → quotes(quote_id), UNIQUE | Accepted quote |
| citizen_id | UUID | FOREIGN KEY → users(user_id) | Project owner |
| provider_id | UUID | FOREIGN KEY → provider_profiles(provider_id) | Service provider |
| project_title | VARCHAR(255) | NOT NULL | Project name |
| project_description | TEXT | NULL | Detailed description |
| status | VARCHAR(50) | DEFAULT 'scheduled' | 'scheduled', 'in_progress', 'completed', 'cancelled', 'disputed' |
| scheduled_start_date | DATE | NULL | Planned start date |
| actual_start_date | DATE | NULL | When work actually started |
| scheduled_end_date | DATE | NULL | Planned completion date |
| actual_completion_date | DATE | NULL | When work was completed |
| agreed_cost | DECIMAL(10,2) | NOT NULL | Original quoted price |
| actual_cost | DECIMAL(10,2) | NULL | Final cost (may differ) |
| cost_difference_reason | TEXT | NULL | Explanation if cost changed |
| payment_status | VARCHAR(50) | DEFAULT 'pending' | 'pending', 'partial', 'completed', 'refunded' |
| work_notes | JSONB | NULL | Array of timestamped progress notes |
| before_images | JSONB | NULL | Images before work started |
| during_images | JSONB | NULL | Progress images |
| after_images | JSONB | NULL | Completion images |
| technical_report_url | TEXT | NULL | Final technical report document |
| completion_certificate_url | TEXT | NULL | Completion certificate |
| warranty_start_date | DATE | NULL | Warranty period start |
| warranty_end_date | DATE | NULL | Warranty period end |
| citizen_satisfaction | VARCHAR(50) | NULL | 'satisfied', 'neutral', 'unsatisfied' |
| created_at | TIMESTAMP | DEFAULT NOW() | Project creation timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `project_id`
- UNIQUE INDEX on `request_id`
- UNIQUE INDEX on `quote_id`
- INDEX on `citizen_id`
- INDEX on `provider_id`
- INDEX on `status`
- INDEX on `scheduled_start_date`
- INDEX on `actual_completion_date`
- INDEX on `created_at DESC`

**Business Rules:**
- Project created automatically when quote is accepted
- Status progression: scheduled → in_progress → completed
- Cannot delete project, only cancel
- Actual cost can differ from agreed cost (requires explanation)
- Completion triggers notification to citizen for rating
- Warranty period calculated from completion date
- Disputed projects require admin intervention

**Status Workflow:**
```
scheduled → in_progress → completed
    ↓            ↓
cancelled    cancelled
    ↓
disputed (requires resolution)
```

**JSONB Structure - work_notes:**
```json
[
  {
    "timestamp": "2024-11-16T09:00:00Z",
    "author": "provider",
    "note": "Started work. Inspected the problem area.",
    "images": ["https://storage.example.com/notes/img1.jpg"]
  },
  {
    "timestamp": "2024-11-16T14:30:00Z",
    "author": "provider",
    "note": "Replaced damaged pipes. Found additional corrosion.",
    "images": ["https://storage.example.com/notes/img2.jpg"]
  }
]
```

---

### 8. CHAT_MESSAGES Table

**Purpose:** Store conversation history between citizens and providers

**Table Name:** `chat_messages`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| message_id | UUID | PRIMARY KEY | Unique identifier for message |
| project_id | UUID | FOREIGN KEY → projects(project_id) | Related project |
| sender_id | UUID | FOREIGN KEY → users(user_id) | User who sent the message |
| receiver_id | UUID | FOREIGN KEY → users(user_id) | User who receives the message |
| message_text | TEXT | NULL | Message content |
| message_type | VARCHAR(50) | DEFAULT 'text' | 'text', 'image', 'file', 'system' |
| attachments | JSONB | NULL | Array of file URLs |
| is_read | BOOLEAN | DEFAULT FALSE | Read status |
| read_at | TIMESTAMP | NULL | When message was read |
| is_deleted_by_sender | BOOLEAN | DEFAULT FALSE | Sender deleted message |
| is_deleted_by_receiver | BOOLEAN | DEFAULT FALSE | Receiver deleted message |
| reply_to_message_id | UUID | FOREIGN KEY → chat_messages(message_id), NULL | For threaded replies |
| created_at | TIMESTAMP | DEFAULT NOW() | Message timestamp |

**Indexes:**
- PRIMARY KEY on `message_id`
- INDEX on `project_id, created_at` (for conversation history)
- INDEX on `sender_id`
- INDEX on `receiver_id`
- INDEX on `is_read` (for unread count)
- INDEX on `created_at DESC`

**Business Rules:**
- Messages belong to a project context
- System messages generated automatically (status changes)
- Soft delete (both users must delete to remove from DB)
- Messages retained for 2 years after project completion
- Real-time delivery via WebSocket
- Push notification if receiver offline

**JSONB Structure - attachments:**
```json
[
  {
    "type": "image",
    "url": "https://storage.example.com/chat/img1.jpg",
    "filename": "problem_detail.jpg",
    "size_bytes": 245632,
    "uploaded_at": "2024-11-16T10:30:00Z"
  },
  {
    "type": "document",
    "url": "https://storage.example.com/chat/quote.pdf",
    "filename": "revised_quote.pdf",
    "size_bytes": 89456,
    "uploaded_at": "2024-11-16T11:00:00Z"
  }
]
```

---

### 9. RATINGS Table

**Purpose:** Store provider ratings and reviews from citizens

**Table Name:** `ratings`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| rating_id | UUID | PRIMARY KEY | Unique identifier for rating |
| project_id | UUID | FOREIGN KEY → projects(project_id), UNIQUE | Rated project |
| citizen_id | UUID | FOREIGN KEY → users(user_id) | User giving rating |
| provider_id | UUID | FOREIGN KEY → provider_profiles(provider_id) | Rated provider |
| overall_rating | INTEGER | NOT NULL, CHECK (1-5) | Overall satisfaction (1-5 stars) |
| quality_rating | INTEGER | NOT NULL, CHECK (1-5) | Work quality (1-5 stars) |
| timeliness_rating | INTEGER | NOT NULL, CHECK (1-5) | On-time completion (1-5 stars) |
| professionalism_rating | INTEGER | NOT NULL, CHECK (1-5) | Professional behavior (1-5 stars) |
| value_rating | INTEGER | NOT NULL, CHECK (1-5) | Value for money (1-5 stars) |
| communication_rating | INTEGER | NOT NULL, CHECK (1-5) | Communication quality (1-5 stars) |
| review_title | VARCHAR(255) | NULL | Short review headline |
| review_text | TEXT | NULL | Detailed review |
| would_recommend | BOOLEAN | NULL | Would recommend to others |
| response_from_provider | TEXT | NULL | Provider's response to review |
| response_at | TIMESTAMP | NULL | When provider responded |
| is_verified | BOOLEAN | DEFAULT TRUE | Verified project completion |
| is_featured | BOOLEAN | DEFAULT FALSE | Featured review |
| helpful_count | INTEGER | DEFAULT 0 | How many found review helpful |
| created_at | TIMESTAMP | DEFAULT NOW() | Rating submission timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `rating_id`
- UNIQUE INDEX on `project_id`
- INDEX on `citizen_id`
- INDEX on `provider_id`
- INDEX on `overall_rating DESC`
- INDEX on `created_at DESC`
- INDEX on `is_featured`

**Business Rules:**
- One rating per project
- Can only rate after project completion
- All rating fields required (1-5 scale)
- Review text optional but encouraged
- Cannot modify rating after 30 days
- Provider can respond once to review
- Average ratings calculated via trigger
- Featured reviews shown prominently

**Calculated Metrics:**
- Average overall rating per provider
- Rating distribution (5-star, 4-star, etc.)
- Trend over time (improving/declining)

---

### 10. PUBLIC_REPORTS Table

**Purpose:** Store citizen reports of public infrastructure issues

**Table Name:** `public_reports`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| report_id | UUID | PRIMARY KEY | Unique identifier for report |
| citizen_id | UUID | FOREIGN KEY → users(user_id) | User submitting report |
| report_type | VARCHAR(100) | NOT NULL | 'road', 'sewage', 'lighting', 'parks', 'waste', 'water', 'other' |
| report_title | VARCHAR(255) | NOT NULL | Brief description |
| report_description | TEXT | NOT NULL | Detailed description |
| images | JSONB | NULL | Array of image URLs |
| latitude | DECIMAL(10,8) | NOT NULL | Geographic latitude |
| longitude | DECIMAL(11,8) | NOT NULL | Geographic longitude |
| location_accuracy | DECIMAL(6,2) | NULL | GPS accuracy in meters |
| address | TEXT | NULL | Human-readable address |
| landmark | VARCHAR(255) | NULL | Nearby landmark |
| status | VARCHAR(50) | DEFAULT 'new' | 'new', 'under_review', 'assigned', 'in_progress', 'resolved', 'closed', 'rejected' |
| priority | VARCHAR(50) | DEFAULT 'medium' | 'low', 'medium', 'high', 'urgent' |
| severity | VARCHAR(50) | NULL | 'minor', 'moderate', 'severe', 'critical' |
| assigned_department | VARCHAR(255) | NULL | Government department handling this |
| assigned_to_user_id | UUID | FOREIGN KEY → users(user_id), NULL | Specific government employee |
| estimated_resolution_date | DATE | NULL | Expected fix date |
| actual_resolution_date | DATE | NULL | When actually resolved |
| government_notes | JSONB | NULL | Internal notes (not visible to citizen) |
| public_updates | JSONB | NULL | Status updates visible to citizen |
| upvotes_count | INTEGER | DEFAULT 0 | How many citizens upvoted |
| views_count | INTEGER | DEFAULT 0 | How many times viewed |
| is_duplicate | BOOLEAN | DEFAULT FALSE | Marked as duplicate of another report |
| duplicate_of_report_id | UUID | FOREIGN KEY → public_reports(report_id), NULL | Original report if duplicate |
| is_verified | BOOLEAN | DEFAULT FALSE | Government verified the issue exists |
| created_at | TIMESTAMP | DEFAULT NOW() | Report submission timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `report_id`
- INDEX on `citizen_id`
- INDEX on `report_type`
- INDEX on `status`
- INDEX on `priority`
- SPATIAL INDEX on `(latitude, longitude)` (PostGIS)
- INDEX on `assigned_department`
- INDEX on `created_at DESC`
- INDEX on `is_duplicate`

**Business Rules:**
- Location (lat/long) required for mapping
- Images optional but encouraged
- Status workflow enforced
- Priority can be upgraded by government
- Duplicate reports linked to original
- Citizens receive notifications on status changes
- Reports archived after 2 years if resolved

**Status Workflow:**
```
new → under_review → assigned → in_progress → resolved → closed
  ↓
rejected (with reason)
```

**JSONB Structure - public_updates:**
```json
[
  {
    "timestamp": "2024-11-16T10:00:00Z",
    "status": "under_review",
    "message": "Your report has been received and is being reviewed.",
    "updated_by": "System"
  },
  {
    "timestamp": "2024-11-17T14:30:00Z",
    "status": "assigned",
    "message": "Report assigned to Roads Department. Work scheduled for next week.",
    "updated_by": "Municipal Office"
  },
  {
    "timestamp": "2024-11-20T09:00:00Z",
    "status": "in_progress",
    "message": "Repair work has started.",
    "updated_by": "Roads Department"
  }
]
```

---

### 11. REPORT_UPDATES Table

**Purpose:** Track all status changes and actions on public reports

**Table Name:** `report_updates`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| update_id | UUID | PRIMARY KEY | Unique identifier for update |
| report_id | UUID | FOREIGN KEY → public_reports(report_id) | Related report |
| updated_by_user_id | UUID | FOREIGN KEY → users(user_id) | Government user who made update |
| old_status | VARCHAR(50) | NULL | Previous status |
| new_status | VARCHAR(50) | NOT NULL | New status |
| old_priority | VARCHAR(50) | NULL | Previous priority |
| new_priority | VARCHAR(50) | NULL | New priority |
| update_type | VARCHAR(50) | NOT NULL | 'status_change', 'assignment', 'note', 'resolution' |
| update_message | TEXT | NULL | Public message to citizen |
| internal_notes | TEXT | NULL | Internal notes (not visible to citizen) |
| attachments | JSONB | NULL | Supporting documents or images |
| notification_sent | BOOLEAN | DEFAULT FALSE | Whether citizen was notified |
| created_at | TIMESTAMP | DEFAULT NOW() | Update timestamp |

**Indexes:**
- PRIMARY KEY on `update_id`
- INDEX on `report_id, created_at` (for update history)
- INDEX on `updated_by_user_id`
- INDEX on `new_status`
- INDEX on `created_at DESC`

**Business Rules:**
- Every status change creates an update record
- Citizen notified automatically for status changes
- Internal notes only visible to government users
- Complete audit trail of all report actions

---

### 12. GOVERNMENT_ANNOUNCEMENTS Table

**Purpose:** Store public announcements from government entities

**Table Name:** `government_announcements`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| announcement_id | UUID | PRIMARY KEY | Unique identifier for announcement |
| created_by_user_id | UUID | FOREIGN KEY → users(user_id) | Government user who created it |
| title | VARCHAR(255) | NOT NULL | Announcement title |
| description | TEXT | NOT NULL | Detailed information |
| announcement_type | VARCHAR(50) | NOT NULL | 'road_closure', 'maintenance', 'warning', 'information', 'emergency' |
| severity | VARCHAR(50) | DEFAULT 'info' | 'info', 'warning', 'critical' |
| affected_area_type | VARCHAR(50) | NULL | 'point', 'circle', 'polygon' |
| center_latitude | DECIMAL(10,8) | NULL | Center point latitude |
| center_longitude | DECIMAL(11,8) | NULL | Center point longitude |
| radius_meters | INTEGER | NULL | Radius for circular area |
| polygon_coordinates | JSONB | NULL | Polygon boundary coordinates |
| affected_districts | JSONB | NULL | Array of district names |
| start_date | TIMESTAMP | NOT NULL | When announcement becomes active |
| end_date | TIMESTAMP | NULL | When announcement expires |
| is_active | BOOLEAN | DEFAULT TRUE | Currently active |
| priority | INTEGER | DEFAULT 0 | Display priority (higher = more important) |
| icon_type | VARCHAR(50) | NULL | Icon to display on map |
| color | VARCHAR(20) | NULL | Color code for map display |
| contact_info | JSONB | NULL | Contact details for inquiries |
| attachments | JSONB | NULL | Related documents or images |
| views_count | INTEGER | DEFAULT 0 | How many times viewed |
| created_at | TIMESTAMP | DEFAULT NOW() | Creation timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `announcement_id`
- INDEX on `created_by_user_id`
- INDEX on `announcement_type`
- INDEX on `is_active`
- INDEX on `start_date, end_date`
- SPATIAL INDEX on `(center_latitude, center_longitude)`
- INDEX on `priority DESC`

**Business Rules:**
- Announcements automatically deactivate after end_date
- Geographic area required for map display
- Emergency announcements shown prominently
- Citizens in affected area receive push notifications
- Expired announcements archived but not deleted

**JSONB Structure - polygon_coordinates:**
```json
{
  "type": "Polygon",
  "coordinates": [
    [
      [46.6753, 24.7136],
      [46.6800, 24.7136],
      [46.6800, 24.7180],
      [46.6753, 24.7180],
      [46.6753, 24.7136]
    ]
  ]
}
```

---

### 13. NOTIFICATIONS Table

**Purpose:** Store user notifications for all events

**Table Name:** `notifications`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| notification_id | UUID | PRIMARY KEY | Unique identifier for notification |
| user_id | UUID | FOREIGN KEY → users(user_id) | Recipient user |
| notification_type | VARCHAR(50) | NOT NULL | Type of notification |
| title | VARCHAR(255) | NOT NULL | Notification title |
| message | TEXT | NOT NULL | Notification content |
| related_entity_type | VARCHAR(50) | NULL | 'project', 'quote', 'report', 'message', etc. |
| related_entity_id | UUID | NULL | ID of related entity |
| action_url | TEXT | NULL | Deep link to relevant page |
| priority | VARCHAR(50) | DEFAULT 'normal' | 'low', 'normal', 'high', 'urgent' |
| delivery_method | JSONB | NULL | How notification was sent |
| is_read | BOOLEAN | DEFAULT FALSE | Read status |
| read_at | TIMESTAMP | NULL | When notification was read |
| is_deleted | BOOLEAN | DEFAULT FALSE | User deleted notification |
| expires_at | TIMESTAMP | NULL | Auto-delete after this date |
| created_at | TIMESTAMP | DEFAULT NOW() | Notification timestamp |

**Indexes:**
- PRIMARY KEY on `notification_id`
- INDEX on `user_id, created_at DESC` (for user's notification list)
- INDEX on `is_read`
- INDEX on `notification_type`
- INDEX on `created_at DESC`
- INDEX on `expires_at` (for cleanup)

**Business Rules:**
- Notifications auto-expire after 90 days
- Unread notifications shown in header badge
- Push notifications sent for high/urgent priority
- Email digest sent for accumulated notifications
- Deleted notifications soft-deleted for 30 days

**Notification Types:**
- `new_quote_received` - Citizen receives quote
- `quote_accepted` - Provider's quote accepted
- `project_status_changed` - Project status update
- `new_message` - New chat message
- `report_status_changed` - Public report update
- `rating_received` - Provider received rating
- `payment_received` - Payment processed
- `system_announcement` - System-wide announcement

**JSONB Structure - delivery_method:**
```json
{
  "push": {
    "sent": true,
    "sent_at": "2024-11-16T10:30:00Z",
    "device_tokens": ["token1", "token2"]
  },
  "email": {
    "sent": true,
    "sent_at": "2024-11-16T10:30:05Z",
    "email_address": "user@example.com"
  },
  "sms": {
    "sent": false,
    "reason": "user_opted_out"
  }
}
```

---

### 14. AUDIT_LOGS Table

**Purpose:** Comprehensive system activity logging for security and compliance

**Table Name:** `audit_logs`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| log_id | UUID | PRIMARY KEY | Unique identifier for log entry |
| user_id | UUID | FOREIGN KEY → users(user_id), NULL | User who performed action (NULL for system) |
| action_type | VARCHAR(50) | NOT NULL | Type of action performed |
| entity_type | VARCHAR(50) | NULL | Type of entity affected |
| entity_id | UUID | NULL | ID of affected entity |
| action_description | TEXT | NULL | Human-readable description |
| old_values | JSONB | NULL | Data before change |
| new_values | JSONB | NULL | Data after change |
| ip_address | VARCHAR(45) | NULL | User's IP address (IPv4 or IPv6) |
| user_agent | TEXT | NULL | Browser/device information |
| request_method | VARCHAR(10) | NULL | HTTP method (GET, POST, etc.) |
| request_url | TEXT | NULL | API endpoint called |
| response_status | INTEGER | NULL | HTTP response code |
| execution_time_ms | INTEGER | NULL | Time taken to process |
| error_message | TEXT | NULL | Error details if action failed |
| session_id | VARCHAR(255) | NULL | User session identifier |
| created_at | TIMESTAMP | DEFAULT NOW() | Log timestamp |

**Indexes:**
- PRIMARY KEY on `log_id`
- INDEX on `user_id`
- INDEX on `action_type`
- INDEX on `entity_type, entity_id`
- INDEX on `created_at DESC`
- INDEX on `ip_address`

**Business Rules:**
- All sensitive operations must be logged
- Logs are immutable (cannot be modified or deleted)
- Retained for minimum 1 year (compliance)
- Automated monitoring for suspicious patterns
- Regular review by security team

**Action Types:**
- `user_login` - Successful login
- `user_login_failed` - Failed login attempt
- `user_logout` - User logged out
- `user_created` - New user registration
- `user_updated` - Profile modification
- `user_deleted` - Account deletion
- `password_changed` - Password update
- `project_created` - New project
- `project_status_changed` - Project status update
- `payment_processed` - Payment transaction
- `report_created` - New public report
- `report_status_changed` - Report status update
- `admin_action` - Administrative action
- `data_export` - Data export request
- `permission_changed` - User permission modification

---

### 15. SYSTEM_SETTINGS Table

**Purpose:** Store application configuration and settings

**Table Name:** `system_settings`

**Columns:**

| Column Name | Data Type | Constraints | Description |
|------------|-----------|-------------|-------------|
| setting_id | UUID | PRIMARY KEY | Unique identifier |
| setting_key | VARCHAR(255) | UNIQUE, NOT NULL | Setting identifier |
| setting_value | TEXT | NULL | Setting value |
| setting_type | VARCHAR(50) | NOT NULL | 'string', 'number', 'boolean', 'json' |
| category | VARCHAR(100) | NULL | Setting category |
| description | TEXT | NULL | What this setting controls |
| is_public | BOOLEAN | DEFAULT FALSE | Can be accessed by frontend |
| is_editable | BOOLEAN | DEFAULT TRUE | Can be modified via admin panel |
| default_value | TEXT | NULL | Default value |
| validation_rules | JSONB | NULL | Validation constraints |
| last_modified_by | UUID | FOREIGN KEY → users(user_id), NULL | Admin who last changed it |
| created_at | TIMESTAMP | DEFAULT NOW() | Creation timestamp |
| updated_at | TIMESTAMP | DEFAULT NOW() | Last update timestamp |

**Indexes:**
- PRIMARY KEY on `setting_id`
- UNIQUE INDEX on `setting_key`
- INDEX on `category`
- INDEX on `is_public`

**Example Settings:**
- `ai_model_version` - Current AI model version
- `max_image_upload_size_mb` - Maximum image size
- `quote_validity_days` - Default quote validity period
- `request_expiry_days` - Service request auto-expiry
- `maintenance_mode` - System maintenance flag
- `featured_providers_count` - How many featured providers to show
- `notification_email_enabled` - Email notifications on/off
- `min_rating_for_featured` - Minimum rating to be featured

---

## 🔗 Relationships and Foreign Keys

### Relationship Summary

#### User-Centric Relationships

**users → provider_profiles (1:1)**
- One user can have one provider profile
- Foreign Key: `provider_profiles.user_id` → `users.user_id`
- ON DELETE: CASCADE (delete profile if user deleted)

**users → diagnostics (1:N)**
- One citizen can have many diagnostics
- Foreign Key: `diagnostics.citizen_id` → `users.user_id`
- ON DELETE: SET NULL (preserve diagnostic data)

**users → service_requests (1:N)**
- One citizen can create many service requests
- Foreign Key: `service_requests.citizen_id` → `users.user_id`
- ON DELETE: SET NULL (preserve request data)

**users → public_reports (1:N)**
- One citizen can submit many public reports
- Foreign Key: `public_reports.citizen_id` → `users.user_id`
- ON DELETE: SET NULL (preserve report data)

**users → chat_messages (1:N)**
- One user can send many messages
- Foreign Keys: 
  - `chat_messages.sender_id` → `users.user_id`
  - `chat_messages.receiver_id` → `users.user_id`
- ON DELETE: SET NULL (preserve message history)

**users → notifications (1:N)**
- One user receives many notifications
- Foreign Key: `notifications.user_id` → `users.user_id`
- ON DELETE: CASCADE (delete notifications with user)

**users → audit_logs (1:N)**
- One user generates many audit log entries
- Foreign Key: `audit_logs.user_id` → `users.user_id`
- ON DELETE: SET NULL (preserve audit trail)

#### Provider-Centric Relationships

**provider_profiles → portfolio_items (1:N)**
- One provider can have many portfolio items
- Foreign Key: `portfolio_items.provider_id` → `provider_profiles.provider_id`
- ON DELETE: CASCADE (delete portfolio with provider)

**provider_profiles → quotes (1:N)**
- One provider can submit many quotes
- Foreign Key: `quotes.provider_id` → `provider_profiles.provider_id`
- ON DELETE: SET NULL (preserve quote history)

**provider_profiles → projects (1:N)**
- One provider can work on many projects
- Foreign Key: `projects.provider_id` → `provider_profiles.provider_id`
- ON DELETE: RESTRICT (cannot delete provider with active projects)

**provider_profiles → ratings (1:N)**
- One provider receives many ratings
- Foreign Key: `ratings.provider_id` → `provider_profiles.provider_id`
- ON DELETE: CASCADE (delete ratings with provider)

#### Service Request Flow Relationships

**diagnostics → service_requests (1:N)**
- One diagnostic can lead to multiple service requests
- Foreign Key: `service_requests.diagnostic_id` → `diagnostics.diagnostic_id`
- ON DELETE: SET NULL (request can exist without diagnostic)

**service_requests → quotes (1:N)**
- One request receives many quotes
- Foreign Key: `quotes.request_id` → `service_requests.request_id`
- ON DELETE: CASCADE (delete quotes if request deleted)

**service_requests → projects (1:1)**
- One accepted request becomes one project
- Foreign Key: `projects.request_id` → `service_requests.request_id`
- ON DELETE: RESTRICT (cannot delete request with active project)

**quotes → projects (1:1)**
- One accepted quote becomes one project
- Foreign Key: `projects.quote_id` → `quotes.quote_id`
- ON DELETE: RESTRICT (cannot delete quote with active project)

#### Project-Centric Relationships

**projects → chat_messages (1:N)**
- One project has many chat messages
- Foreign Key: `chat_messages.project_id` → `projects.project_id`
- ON DELETE: CASCADE (delete messages with project)

**projects → ratings (1:1)**
- One project receives one rating
- Foreign Key: `ratings.project_id` → `projects.project_id`
- ON DELETE: CASCADE (delete rating with project)

#### Public Reporting Relationships

**public_reports → report_updates (1:N)**
- One report has many status updates
- Foreign Key: `report_updates.report_id` → `public_reports.report_id`
- ON DELETE: CASCADE (delete updates with report)

**public_reports → public_reports (1:N) - Self-Referencing**
- One report can have many duplicates
- Foreign Key: `public_reports.duplicate_of_report_id` → `public_reports.report_id`
- ON DELETE: SET NULL (unlink duplicates if original deleted)

### Foreign Key Constraints Summary

**CASCADE:**
- Delete child records when parent is deleted
- Used for dependent data (portfolio items, notifications)

**SET NULL:**
- Set foreign key to NULL when parent deleted
- Used to preserve historical data (diagnostics, requests)

**RESTRICT:**
- Prevent deletion of parent if children exist
- Used for critical relationships (active projects)

**NO ACTION:**
- Similar to RESTRICT but checked at end of transaction
- Used for complex multi-table operations

---

## 📑 Indexing Strategy

### Index Types

#### 1. Primary Key Indexes
**Automatically Created:**
- Every table has PRIMARY KEY index on ID column
- Uses B-tree index structure
- Ensures uniqueness and fast lookups

#### 2. Foreign Key Indexes
**Manually Created:**
- Index all foreign key columns
- Speeds up JOIN operations
- Improves referential integrity checks

**Example:**
```
CREATE INDEX idx_diagnostics_citizen_id ON diagnostics(citizen_id);
CREATE INDEX idx_projects_provider_id ON projects(provider_id);
```

#### 3. Unique Indexes
**For Unique Constraints:**
- Email addresses
- One-to-one relationships
- Prevent duplicate entries

**Example:**
```
CREATE UNIQUE INDEX idx_users_email ON users(email);
CREATE UNIQUE INDEX idx_provider_profiles_user_id ON provider_profiles(user_id);
```

#### 4. Composite Indexes
**Multiple Columns:**
- For queries filtering on multiple columns
- Order matters (most selective first)

**Example:**
```
CREATE INDEX idx_projects_status_date ON projects(status, scheduled_start_date);
CREATE INDEX idx_public_reports_type_status ON public_reports(report_type, status);
```

#### 5. Partial Indexes
**Filtered Indexes:**
- Index only subset of rows
- Smaller, faster indexes

**Example:**
```
CREATE INDEX idx_active_users ON users(user_id) WHERE is_active = TRUE;
CREATE INDEX idx_open_requests ON service_requests(request_id) WHERE status = 'open';
```

#### 6. JSONB Indexes (GIN)
**For JSON Columns:**
- Enable fast searches within JSON data
- Support containment and existence operators

**Example:**
```
CREATE INDEX idx_provider_services ON provider_profiles USING GIN(services_offered);
CREATE INDEX idx_project_notes ON projects USING GIN(work_notes);
```

#### 7. Full-Text Search Indexes (GIN)
**For Text Search:**
- Search through descriptions and reviews
- Language-aware tokenization

**Example:**
```
CREATE INDEX idx_diagnostics_cause_search ON diagnostics USING GIN(to_tsvector('english', probable_cause));
CREATE INDEX idx_ratings_review_search ON ratings USING GIN(to_tsvector('english', review_text));
```

#### 8. Geospatial Indexes (GiST)
**For Location Data:**
- Fast proximity searches
- Requires PostGIS extension

**Example:**
```
CREATE INDEX idx_public_reports_location ON public_reports USING GIST(ST_MakePoint(longitude, latitude));
```

### Indexing Best Practices

**Do Index:**
- Primary keys (automatic)
- Foreign keys (always)
- Columns in WHERE clauses
- Columns in ORDER BY clauses
- Columns in JOIN conditions
- Columns with high selectivity

**Don't Over-Index:**
- Small tables (<1000 rows)
- Columns that change frequently
- Columns with low selectivity (boolean with 50/50 distribution)
- Every column "just in case"

**Index Maintenance:**
- Monitor index usage with `pg_stat_user_indexes`
- Remove unused indexes
- Rebuild fragmented indexes periodically
- Update statistics with ANALYZE

---

## 🛡️ Data Integrity and Constraints

### Primary Key Constraints
**Every Table:**
- UUID primary key
- Ensures unique identification
- Prevents duplicate records

### Foreign Key Constraints
**Referential Integrity:**
- Child records must reference existing parent
- Cascade or restrict deletes appropriately
- Maintain data consistency

### Check Constraints

**Rating Values:**
```sql
ALTER TABLE ratings ADD CONSTRAINT chk_overall_rating 
  CHECK (overall_rating >= 1 AND overall_rating <= 5);
```

**Status Values:**
```sql
ALTER TABLE projects ADD CONSTRAINT chk_project_status 
  CHECK (status IN ('scheduled', 'in_progress', 'completed', 'cancelled', 'disputed'));
```

**Date Logic:**
```sql
ALTER TABLE projects ADD CONSTRAINT chk_dates 
  CHECK (scheduled_end_date >= scheduled_start_date);
```

**Cost Values:**
```sql
ALTER TABLE quotes ADD CONSTRAINT chk_positive_cost 
  CHECK (estimated_cost > 0);
```

### Unique Constraints

**Prevent Duplicates:**
- One provider profile per user
- One rating per project
- One quote per provider per request
- Unique email addresses

### Not Null Constraints

**Required Fields:**
- User email and password
- Project status
- Report location
- Rating values

### Default Values

**Sensible Defaults:**
- Timestamps (NOW())
- Boolean flags (FALSE)
- Status fields ('pending', 'new')
- Counters (0)

### Database Triggers

#### 1. Update Timestamp Trigger
**Auto-update updated_at:**
```
Trigger: update_timestamp
On: UPDATE of any table with updated_at column
Action: SET updated_at = NOW()
```

#### 2. Rating Calculation Trigger
**Update provider average rating:**
```
Trigger: calculate_provider_rating
On: INSERT or UPDATE on ratings table
Action: Recalculate provider_profiles.average_rating
```

#### 3. Project Count Trigger
**Update provider project count:**
```
Trigger: update_project_count
On: UPDATE of projects.status to 'completed'
Action: INCREMENT provider_profiles.total_projects
```

#### 4. Quote Count Trigger
**Update request quote count:**
```
Trigger: update_quote_count
On: INSERT on quotes table
Action: INCREMENT service_requests.quotes_count
```

#### 5. Notification Trigger
**Auto-create notifications:**
```
Trigger: create_notification
On: Status changes in projects or reports
Action: INSERT into notifications table
```

---

## 🔐 Database Security

### Access Control

#### Role-Based Database Access

**Application Role:**
- Read/Write access to all application tables
- No access to system tables
- Connection from application servers only

**Admin Role:**
- Full access to all tables
- Can create/modify schema
- Can view audit logs
- Access from admin tools only

**Backup Role:**
- Read-only access
- Can perform backups
- No write permissions

**Analytics Role:**
- Read-only access to specific tables
- For reporting and analytics
- No access to sensitive fields

### Connection Security

**SSL/TLS Encryption:**
- All database connections encrypted
- Certificate-based authentication
- Minimum TLS 1.2

**Connection Limits:**
- Maximum connections per role
- Idle connection timeout
- Connection pooling

**IP Whitelisting:**
- Allow connections only from known IPs
- Application servers
- Admin machines
- VPN for remote access

### Data Encryption

**Encryption at Rest:**
- Full database encryption
- Encrypted backups
- Secure key management

**Column-Level Encryption:**
- Sensitive fields encrypted
- Password hashes (bcrypt)
- Payment information (if applicable)

**Transparent Data Encryption (TDE):**
- Automatic encryption/decryption
- No application changes needed
- Managed by database engine

### SQL Injection Prevention

**Parameterized Queries:**
- Always use prepared statements
- Never concatenate user input
- ORM handles automatically

**Input Validation:**
- Validate at application layer
- Sanitize user input
- Type checking

**Least Privilege:**
- Application user has minimum permissions
- No DROP, TRUNCATE permissions
- Read-only where possible

### Audit and Monitoring

**Database Audit Logging:**
- Log all DDL statements
- Log privileged operations
- Log failed authentication attempts
- Log data exports

**Query Monitoring:**
- Slow query logging
- Suspicious query patterns
- Unusual access patterns
- Failed query attempts

**Alerting:**
- Multiple failed logins
- Schema changes
- Large data exports
- Performance degradation

---

## 💾 Backup and Recovery

### Backup Strategy

#### Full Backups
**Frequency:** Daily at 2:00 AM
**Retention:** 30 days
**Method:** pg_dump or cloud provider snapshots
**Storage:** Encrypted cloud storage

#### Incremental Backups
**Frequency:** Every 6 hours
**Retention:** 7 days
**Method:** Write-Ahead Log (WAL) archiving
**Storage:** Separate storage location

#### Transaction Log Backups
**Frequency:** Continuous
**Retention:** Until full backup
**Method:** WAL streaming
**Purpose:** Point-in-time recovery

### Backup Verification

**Automated Testing:**
- Weekly restore test
- Verify data integrity
- Check backup completeness
- Document results

**Monitoring:**
- Backup success/failure alerts
- Storage capacity monitoring
- Backup duration tracking
- Restore time estimation

### Disaster Recovery Plan

**Recovery Time Objective (RTO):** 4 hours
**Recovery Point Objective (RPO):** 15 minutes

**Recovery Procedures:**

**1. Minor Data Loss (< 1 hour):**
- Restore from transaction logs
- Minimal downtime
- Automated process

**2. Database Corruption:**
- Restore from latest full backup
- Apply transaction logs
- Verify data integrity
- Estimated time: 2-4 hours

**3. Complete System Failure:**
- Failover to backup region
- Restore from replicated backup
- Update DNS records
- Estimated time: 1-2 hours

**4. Ransomware/Security Breach:**
- Isolate affected systems
- Restore from clean backup
- Security audit
- Estimated time: 4-8 hours

### High Availability

**Database Replication:**
- Primary-Replica setup
- Synchronous replication for critical data
- Asynchronous replication for analytics
- Automatic failover

**Load Balancing:**
- Read replicas for SELECT queries
- Write operations to primary only
- Connection pooling
- Health checks

---

## ⚡ Performance Optimization

### Query Optimization

**Techniques:**

**1. Use Indexes Effectively:**
- Index foreign keys
- Index WHERE clause columns
- Composite indexes for multiple columns
- Monitor index usage

**2. Optimize JOINs:**
- Join on indexed columns
- Use appropriate JOIN types
- Avoid unnecessary JOINs
- Consider denormalization for frequently joined tables

**3. Limit Result Sets:**
- Use LIMIT and OFFSET for pagination
- Filter early with WHERE clauses
- Select only needed columns (avoid SELECT *)
- Use covering indexes

**4. Avoid N+1 Queries:**
- Use JOINs instead of multiple queries
- Batch loading with IN clauses
- ORM eager loading

**5. Use EXPLAIN ANALYZE:**
- Understand query execution plans
- Identify bottlenecks
- Optimize slow queries
- Monitor query performance

### Database Design Optimization

**Normalization vs. Denormalization:**

**Normalized (Current Design):**
- Eliminates redundancy
- Maintains data integrity
- Requires JOINs for queries
- Good for write-heavy operations

**Strategic Denormalization:**
- Store calculated values (average_rating, total_projects)
- Cache frequently accessed data
- Reduce JOIN complexity
- Good for read-heavy operations

**Materialized Views:**
- Pre-computed complex queries
- Refresh periodically
- Fast read access
- Used for analytics and reports

**Example:**
```
Materialized View: provider_statistics
- Aggregates ratings, projects, completion rates
- Refreshed nightly
- Fast provider listing queries
```

### Caching Strategy

**Application-Level Cache (Redis):**

**What to Cache:**
- User sessions
- Provider profiles (frequently viewed)
- Average ratings
- Public report counts
- System settings

**Cache Invalidation:**
- Time-based expiration (TTL)
- Event-based invalidation (on update)
- Cache-aside pattern
- Write-through cache

**Database-Level Cache:**
- PostgreSQL shared buffers
- Query result cache
- Connection pooling

### Partitioning

**Table Partitioning:**

**When to Partition:**
- Tables > 100GB
- Time-series data
- Improve query performance
- Easier data archival

**Partition Strategies:**

**1. Time-Based Partitioning:**
```
audit_logs table partitioned by month
- audit_logs_2024_11
- audit_logs_2024_12
- audit_logs_2025_01
```

**2. Range Partitioning:**
```
projects table partitioned by status
- projects_active (scheduled, in_progress)
- projects_completed
- projects_archived
```

**Benefits:**
- Faster queries (scan only relevant partitions)
- Easier maintenance (drop old partitions)
- Parallel query execution
- Improved backup/restore

### Connection Pooling

**PgBouncer or Application-Level Pooling:**
- Reuse database connections
- Reduce connection overhead
- Handle connection spikes
- Improve scalability

**Pool Configuration:**
- Pool size: 20-50 connections
- Max client connections: 1000
- Idle timeout: 30 seconds
- Connection lifetime: 30 minutes

---

## 📈 Scalability Considerations

### Vertical Scaling (Scale Up)

**Increase Server Resources:**
- More CPU cores (parallel queries)
- More RAM (larger cache)
- Faster storage (SSD, NVMe)
- Better network bandwidth

**Limits:**
- Hardware limits
- Cost increases exponentially
- Single point of failure

### Horizontal Scaling (Scale Out)

**Read Replicas:**
- Multiple read-only copies
- Distribute SELECT queries
- Reduce primary server load
- Geographic distribution

**Sharding (Future):**
- Split data across multiple databases
- By geographic region
- By user ID range
- By provider type

**Challenges:**
- Complex queries across shards
- Data consistency
- Application complexity

### Database Architecture Evolution

**Phase 1: Single Database (Current)**
- Monolithic PostgreSQL instance
- Suitable for 0-100K users
- Simple architecture
- Easy to manage

**Phase 2: Primary-Replica Setup**
- One primary (writes)
- Multiple replicas (reads)
- Suitable for 100K-1M users
- Improved read performance

**Phase 3: Sharded Architecture**
- Multiple database clusters
- Data partitioned by region or user
- Suitable for 1M+ users
- Complex but highly scalable

**Phase 4: Microservices Databases**
- Separate databases per service
- Service-specific optimization
- Independent scaling
- Maximum flexibility

### Monitoring and Capacity Planning

**Key Metrics to Monitor:**

**Performance Metrics:**
- Query execution time
- Transactions per second (TPS)
- Connection count
- Cache hit ratio
- Index usage

**Resource Metrics:**
- CPU utilization
- Memory usage
- Disk I/O
- Network throughput
- Storage capacity

**Application Metrics:**
- Active users
- API request rate
- Error rate
- Response time

**Alerting Thresholds:**
- CPU > 80% for 5 minutes
- Memory > 90%
- Disk space < 20%
- Query time > 5 seconds
- Connection pool exhausted

**Capacity Planning:**
- Monitor growth trends
- Predict resource needs
- Plan upgrades proactively
- Load testing before scaling

---

## 🔄 Data Migration Strategy

### Initial Database Setup

**1. Schema Creation:**
- Run DDL scripts to create tables
- Create indexes
- Add constraints
- Set up triggers

**2. Seed Data:**
- System settings
- Default categories
- Test users (development)
- Sample data (development)

**3. Verification:**
- Check all tables created
- Verify relationships
- Test constraints
- Run sample queries

### Migration Management

**Migration Tools:**
- **Sequelize Migrations** (Node.js ORM)
- **Prisma Migrate** (Modern ORM)
- **Flyway** (Database migration tool)
- **Liquibase** (Database change management)

**Migration Workflow:**

**1. Development:**
- Create migration file
- Test locally
- Peer review
- Commit to version control

**2. Staging:**
- Run migration on staging database
- Verify data integrity
- Test application
- Performance testing

**3. Production:**
- Schedule maintenance window
- Backup database
- Run migration
- Verify success
- Monitor application
- Rollback plan ready

**Migration Best Practices:**
- Incremental changes (small migrations)
- Backward compatible when possible
- Test rollback procedures
- Document breaking changes
- Version control all migrations

### Data Import/Export

**Import Scenarios:**
- Initial data load
- Bulk user import
- Historical data migration
- Integration with external systems

**Export Scenarios:**
- Data backup
- Analytics export
- Reporting
- Compliance requirements

**Tools:**
- `pg_dump` / `pg_restore` (PostgreSQL native)
- CSV import/export
- Custom ETL scripts
- Data migration services

---

## 📊 Sample Data Scenarios

### Scenario 1: Complete User Journey - Home Maintenance

**Step 1: User Registration**
```
Table: users
- user_id: 550e8400-e29b-41d4-a716-446655440001
- email: sara.citizen@example.com
- role: citizen
- full_name: Sara Ahmed
```

**Step 2: AI Diagnostic**
```
Table: diagnostics
- diagnostic_id: 660e8400-e29b-41d4-a716-446655440001
- citizen_id: 550e8400-e29b-41d4-a716-446655440001
- image_url: https://storage.example.com/diagnostics/pipe_leak.jpg
- risk_level: high
- problem_category: plumbing
- probable_cause: Corroded copper pipe with visible leak
- ai_confidence_score: 92.5
```

**Step 3: Service Request**
```
Table: service_requests
- request_id: 770e8400-e29b-41d4-a716-446655440001
- diagnostic_id: 660e8400-e29b-41d4-a716-446655440001
- citizen_id: 550e8400-e29b-41d4-a716-446655440001
- problem_title: Urgent pipe leak in kitchen
- status: open
```

**Step 4: Provider Submits Quote**
```
Table: quotes
- quote_id: 880e8400-e29b-41d4-a716-446655440001
- request_id: 770e8400-e29b-41d4-a716-446655440001
- provider_id: 990e8400-e29b-41d4-a716-446655440001
- estimated_cost: 850.00
- estimated_duration_days: 1
- status: pending
```

**Step 5: Citizen Accepts Quote**
```
Table: service_requests (UPDATE)
- status: provider_selected
- selected_quote_id: 880e8400-e29b-41d4-a716-446655440001

Table: quotes (UPDATE)
- status: accepted

Table: projects (INSERT)
- project_id: 100e8400-e29b-41d4-a716-446655440001
- request_id: 770e8400-e29b-41d4-a716-446655440001
- quote_id: 880e8400-e29b-41d4-a716-446655440001
- status: scheduled
```

**Step 6: Project Completion**
```
Table: projects (UPDATE)
- status: completed
- actual_completion_date: 2024-11-18
- actual_cost: 850.00
```

**Step 7: Rating**
```
Table: ratings
- rating_id: 110e8400-e29b-41d4-a716-446655440001
- project_id: 100e8400-e29b-41d4-a716-446655440001
- overall_rating: 5
- quality_rating: 5
- timeliness_rating: 5
- review_text: Excellent service, fixed the problem quickly!
```

### Scenario 2: Public Infrastructure Report

**Step 1: Citizen Reports Pothole**
```
Table: public_reports
- report_id: 120e8400-e29b-41d4-a716-446655440001
- citizen_id: 550e8400-e29b-41d4-a716-446655440001
- report_type: road
- report_title: Large pothole on Main Street
- latitude: 24.7136
- longitude: 46.6753
- status: new
- priority: medium
```

**Step 2: Government Reviews**
```
Table: public_reports (UPDATE)
- status: under_review
- assigned_department: Roads Department

Table: report_updates (INSERT)
- update_id: 130e8400-e29b-41d4-a716-446655440001
- report_id: 120e8400-e29b-41d4-a716-446655440001
- new_status: under_review
- update_message: Report received and being reviewed
```

**Step 3: Work Scheduled**
```
Table: public_reports (UPDATE)
- status: in_progress
- priority: high
- estimated_resolution_date: 2024-11-25

Table: report_updates (INSERT)
- new_status: in_progress
- update_message: Repair work scheduled for next week
```

**Step 4: Issue Resolved**
```
Table: public_reports (UPDATE)
- status: resolved
- actual_resolution_date: 2024-11-24

Table: report_updates (INSERT)
- new_status: resolved
- update_message: Pothole has been repaired. Thank you for reporting!
```

### Scenario 3: Provider Profile Setup

**Step 1: Engineer Registration**
```
Table: users
- user_id: 990e8400-e29b-41d4-a716-446655440001
- email: mohammed.engineer@example.com
- role: engineer
- full_name: Mohammed Hassan
```

**Step 2: Provider Profile**
```
Table: provider_profiles
- provider_id: 990e8400-e29b-41d4-a716-446655440001
- user_id: 990e8400-e29b-41d4-a716-446655440001
- provider_type: engineer
- business_name: Hassan Plumbing Services
- services_offered: ["plumbing", "hvac"]
- coverage_areas: [{"city": "Riyadh", "districts": ["Al Olaya", "Al Malqa"]}]
- years_of_experience: 10
```

**Step 3: Portfolio Items**
```
Table: portfolio_items
- portfolio_id: 140e8400-e29b-41d4-a716-446655440001
- provider_id: 990e8400-e29b-41d4-a716-446655440001
- title: Complete bathroom renovation
- project_type: plumbing
- images: [{"url": "...", "caption": "Before"}, {"url": "...", "caption": "After"}]
```

---

## 📝 Database Documentation Standards

### Documentation Requirements

**For Each Table:**
- Purpose and description
- Column definitions
- Data types and constraints
- Relationships
- Indexes
- Business rules
- Sample data

**For Each Relationship:**
- Parent and child tables
- Foreign key columns
- Cascade rules
- Business logic

**For Each Index:**
- Purpose
- Columns included
- Index type
- Performance impact

### Change Management

**When Modifying Database:**
1. Document reason for change
2. Create migration script
3. Update ERD diagram
4. Update this wiki
5. Test thoroughly
6. Peer review
7. Deploy with rollback plan

### Naming Conventions

**Tables:**
- Plural nouns (users, projects)
- Lowercase with underscores (public_reports)
- Descriptive names

**Columns:**
- Singular nouns (user_id, email)
- Lowercase with underscores
- Suffix _id for foreign keys
- Suffix _at for timestamps
- Suffix _count for counters

**Indexes:**
- Prefix idx_ (idx_users_email)
- Include table and column names
- Descriptive of purpose

**Constraints:**
- Prefix chk_ for CHECK constraints
- Prefix fk_ for FOREIGN KEY constraints
- Prefix uq_ for UNIQUE constraints

---

## 🎯 Conclusion

This database design provides a solid foundation for the Smart Home Maintenance & Public Infrastructure Platform. Key strengths:

**Comprehensive Coverage:**
- All user roles supported
- Complete feature set
- Extensible design

**Data Integrity:**
- Strong relationships
- Appropriate constraints
- Audit trail

**Performance:**
- Strategic indexing
- Caching strategy
- Scalability path

**Security:**
- Access control
- Encryption
- Audit logging

**Maintainability:**
- Clear structure
- Good documentation
- Migration strategy

The design balances normalization for data integrity with strategic denormalization for performance, providing an efficient and scalable database architecture for your graduation project.

---

*Document Version: 1.0*  
*Last Updated: November 2025*  
*Database Type: PostgreSQL 14+*  
*Total Tables: 15*  
*Estimated Database Size: 10-50 GB (first year)*

