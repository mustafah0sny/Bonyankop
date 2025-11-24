-- =====================================================
-- Smart Home Maintenance Platform - Database Setup Script
-- PostgreSQL 14+ Compatible
-- Version: 3.0
-- Last Updated: November 2025
-- =====================================================

-- =====================================================
-- DATABASE CREATION (Run as superuser)
-- =====================================================

-- Terminate existing connections to the database
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'smart_home_maintenance'
  AND pid <> pg_backend_pid();

-- Drop database if exists (CAUTION: This will delete all data!)
DROP DATABASE IF EXISTS smart_home_maintenance;

-- Create database
CREATE DATABASE smart_home_maintenance
    WITH 
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    TEMPLATE = template0
    CONNECTION LIMIT = -1;

-- Connect to the database
\c smart_home_maintenance

-- =====================================================
-- EXTENSIONS
-- =====================================================

-- UUID generation
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Full-text search
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- =====================================================
-- CUSTOM TYPES (ENUMS)
-- =====================================================

-- User roles
CREATE TYPE user_role AS ENUM (
    'citizen',
    'engineer',
    'company',
    'government',
    'admin'
);

-- Provider types
CREATE TYPE provider_type AS ENUM (
    'company',
    'engineer'
);

-- Risk levels
CREATE TYPE risk_level AS ENUM (
    'low',
    'medium',
    'high'
);

-- Problem categories
CREATE TYPE problem_category AS ENUM (
    'plumbing',
    'electrical',
    'structural',
    'hvac',
    'roofing'
);

-- Service request status
CREATE TYPE request_status AS ENUM (
    'open',
    'quotes_received',
    'provider_selected',
    'cancelled'
);

-- Quote status
CREATE TYPE quote_status AS ENUM (
    'pending',
    'accepted',
    'rejected',
    'expired',
    'withdrawn'
);

-- Project status
CREATE TYPE project_status AS ENUM (
    'scheduled',
    'in_progress',
    'completed',
    'cancelled',
    'disputed'
);

-- Payment status
CREATE TYPE payment_status AS ENUM (
    'pending',
    'partial',
    'completed',
    'refunded'
);

-- =====================================================
-- TABLES
-- =====================================================

-- -----------------------------------------------------
-- Table: users
-- -----------------------------------------------------
CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role user_role NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    phone_number VARCHAR(20),
    profile_picture_url TEXT,
    is_verified BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    last_login_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT email_format_check CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    CONSTRAINT phone_format_check CHECK (phone_number IS NULL OR phone_number ~* '^\+?[0-9\s\-\(\)]{10,20}$')
);

-- Indexes for users table
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_active_role ON users(is_active, role);
CREATE INDEX idx_users_created_at ON users(created_at);

-- Comments
COMMENT ON TABLE users IS 'Core user accounts for all platform users';
COMMENT ON COLUMN users.role IS 'User role: citizen, engineer, company, government, or admin';

-- -----------------------------------------------------
-- Table: provider_profiles
-- -----------------------------------------------------
CREATE TABLE provider_profiles (
    provider_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID UNIQUE NOT NULL,
    provider_type provider_type NOT NULL,
    business_name VARCHAR(255) NOT NULL,
    description TEXT,
    services_offered JSONB,
    certifications JSONB,
    coverage_areas JSONB,
    license_number VARCHAR(100),
    years_of_experience INTEGER CHECK (years_of_experience >= 0),
    average_rating DECIMAL(3,2) DEFAULT 0.00 CHECK (average_rating >= 0 AND average_rating <= 5),
    total_projects INTEGER DEFAULT 0 CHECK (total_projects >= 0),
    total_ratings INTEGER DEFAULT 0 CHECK (total_ratings >= 0),
    completion_rate DECIMAL(5,2) DEFAULT 0.00 CHECK (completion_rate >= 0 AND completion_rate <= 100),
    response_time_hours DECIMAL(5,2) CHECK (response_time_hours >= 0),
    is_verified BOOLEAN DEFAULT FALSE,
    is_featured BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_provider_user FOREIGN KEY (user_id) 
        REFERENCES users(user_id) ON DELETE CASCADE
);

-- Indexes for provider_profiles table
CREATE INDEX idx_provider_user_id ON provider_profiles(user_id);
CREATE INDEX idx_provider_type ON provider_profiles(provider_type);
CREATE INDEX idx_provider_verified ON provider_profiles(is_verified);
CREATE INDEX idx_provider_rating ON provider_profiles(average_rating DESC);
CREATE INDEX idx_provider_verified_featured ON provider_profiles(is_verified, is_featured);
CREATE INDEX idx_provider_services ON provider_profiles USING GIN (services_offered);

-- Comments
COMMENT ON TABLE provider_profiles IS 'Extended profile for service providers (engineers and companies)';

-- -----------------------------------------------------
-- Table: portfolio_items
-- -----------------------------------------------------
CREATE TABLE portfolio_items (
    portfolio_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    provider_id UUID NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    project_type VARCHAR(100),
    images JSONB,
    project_date DATE,
    location VARCHAR(255),
    display_order INTEGER DEFAULT 0,
    is_featured BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_portfolio_provider FOREIGN KEY (provider_id) 
        REFERENCES provider_profiles(provider_id) ON DELETE CASCADE
);

-- Indexes for portfolio_items table
CREATE INDEX idx_portfolio_provider ON portfolio_items(provider_id);
CREATE INDEX idx_portfolio_provider_order ON portfolio_items(provider_id, display_order);
CREATE INDEX idx_portfolio_featured ON portfolio_items(is_featured);

-- Comments
COMMENT ON TABLE portfolio_items IS 'Showcase of completed work by service providers';

-- -----------------------------------------------------
-- Table: diagnostics
-- -----------------------------------------------------
CREATE TABLE diagnostics (
    diagnostic_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    citizen_id UUID NOT NULL,
    image_url TEXT NOT NULL,
    image_metadata JSONB,
    risk_level risk_level NOT NULL,
    problem_category problem_category NOT NULL,
    problem_subcategory VARCHAR(100),
    probable_cause TEXT NOT NULL,
    risk_prediction TEXT NOT NULL,
    recommended_action TEXT NOT NULL,
    ai_confidence_score DECIMAL(5,2) NOT NULL CHECK (ai_confidence_score >= 0 AND ai_confidence_score <= 100),
    ai_model_version VARCHAR(50),
    processing_time_ms INTEGER CHECK (processing_time_ms >= 0),
    is_diy_possible BOOLEAN DEFAULT FALSE,
    estimated_cost_range VARCHAR(100),
    urgency_level VARCHAR(50),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_diagnostic_citizen FOREIGN KEY (citizen_id) 
        REFERENCES users(user_id) ON DELETE SET NULL
);

-- Indexes for diagnostics table
CREATE INDEX idx_diagnostic_citizen ON diagnostics(citizen_id);
CREATE INDEX idx_diagnostic_category ON diagnostics(problem_category);
CREATE INDEX idx_diagnostic_risk ON diagnostics(risk_level);
CREATE INDEX idx_diagnostic_created ON diagnostics(created_at DESC);
CREATE INDEX idx_diagnostic_citizen_created ON diagnostics(citizen_id, created_at DESC);

-- Comments
COMMENT ON TABLE diagnostics IS 'AI-powered diagnostic analysis of home maintenance issues';

-- -----------------------------------------------------
-- Table: service_requests
-- -----------------------------------------------------
CREATE TABLE service_requests (
    request_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    diagnostic_id UUID,
    citizen_id UUID NOT NULL,
    problem_title VARCHAR(255) NOT NULL,
    problem_description TEXT NOT NULL,
    problem_category VARCHAR(100) NOT NULL,
    additional_images JSONB,
    preferred_provider_type VARCHAR(50),
    preferred_service_date DATE,
    property_type VARCHAR(50),
    property_address TEXT,
    contact_phone VARCHAR(20),
    status request_status DEFAULT 'open' NOT NULL,
    selected_quote_id UUID,
    quotes_count INTEGER DEFAULT 0 CHECK (quotes_count >= 0),
    views_count INTEGER DEFAULT 0 CHECK (views_count >= 0),
    expires_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_request_diagnostic FOREIGN KEY (diagnostic_id) 
        REFERENCES diagnostics(diagnostic_id) ON DELETE SET NULL,
    CONSTRAINT fk_request_citizen FOREIGN KEY (citizen_id) 
        REFERENCES users(user_id) ON DELETE SET NULL
);

-- Indexes for service_requests table
CREATE INDEX idx_request_citizen ON service_requests(citizen_id);
CREATE INDEX idx_request_diagnostic ON service_requests(diagnostic_id);
CREATE INDEX idx_request_status ON service_requests(status);
CREATE INDEX idx_request_category ON service_requests(problem_category);
CREATE INDEX idx_request_created ON service_requests(created_at DESC);
CREATE INDEX idx_request_status_created ON service_requests(status, created_at DESC);
CREATE UNIQUE INDEX idx_request_selected_quote ON service_requests(selected_quote_id) WHERE selected_quote_id IS NOT NULL;

-- Full-text search index
CREATE INDEX idx_request_title_search ON service_requests USING GIN (to_tsvector('english', problem_title));
CREATE INDEX idx_request_description_search ON service_requests USING GIN (to_tsvector('english', problem_description));

-- Comments
COMMENT ON TABLE service_requests IS 'Service requests from citizens seeking professional help';

-- -----------------------------------------------------
-- Table: quotes
-- -----------------------------------------------------
CREATE TABLE quotes (
    quote_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    request_id UUID NOT NULL,
    provider_id UUID NOT NULL,
    estimated_cost DECIMAL(10,2) NOT NULL CHECK (estimated_cost >= 0),
    cost_breakdown JSONB,
    estimated_duration_days INTEGER CHECK (estimated_duration_days > 0),
    technical_assessment TEXT,
    proposed_solution TEXT,
    materials_included BOOLEAN DEFAULT FALSE,
    warranty_period_months INTEGER CHECK (warranty_period_months >= 0),
    terms_and_conditions TEXT,
    validity_period_days INTEGER DEFAULT 7 CHECK (validity_period_days > 0),
    attachments JSONB,
    status quote_status DEFAULT 'pending' NOT NULL,
    rejection_reason TEXT,
    submitted_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    accepted_at TIMESTAMP WITH TIME ZONE,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_quote_request FOREIGN KEY (request_id) 
        REFERENCES service_requests(request_id) ON DELETE CASCADE,
    CONSTRAINT fk_quote_provider FOREIGN KEY (provider_id) 
        REFERENCES provider_profiles(provider_id) ON DELETE SET NULL,
    CONSTRAINT unique_provider_per_request UNIQUE (request_id, provider_id)
);

-- Indexes for quotes table
CREATE INDEX idx_quote_request ON quotes(request_id);
CREATE INDEX idx_quote_provider ON quotes(provider_id);
CREATE INDEX idx_quote_status ON quotes(status);
CREATE INDEX idx_quote_submitted ON quotes(submitted_at DESC);
CREATE INDEX idx_quote_expires ON quotes(expires_at);

-- Comments
COMMENT ON TABLE quotes IS 'Price quotes submitted by service providers';
COMMENT ON CONSTRAINT unique_provider_per_request ON quotes IS 'One quote per provider per request';

-- -----------------------------------------------------
-- Table: projects
-- -----------------------------------------------------
CREATE TABLE projects (
    project_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    request_id UUID UNIQUE NOT NULL,
    quote_id UUID UNIQUE NOT NULL,
    citizen_id UUID NOT NULL,
    provider_id UUID NOT NULL,
    project_title VARCHAR(255) NOT NULL,
    project_description TEXT,
    status project_status DEFAULT 'scheduled' NOT NULL,
    scheduled_start_date DATE,
    actual_start_date DATE,
    scheduled_end_date DATE,
    actual_completion_date DATE,
    agreed_cost DECIMAL(10,2) NOT NULL CHECK (agreed_cost >= 0),
    actual_cost DECIMAL(10,2) CHECK (actual_cost >= 0),
    cost_difference_reason TEXT,
    payment_status payment_status DEFAULT 'pending',
    work_notes JSONB,
    before_images JSONB,
    during_images JSONB,
    after_images JSONB,
    technical_report_url TEXT,
    completion_certificate_url TEXT,
    warranty_start_date DATE,
    warranty_end_date DATE,
    citizen_satisfaction VARCHAR(50),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_project_request FOREIGN KEY (request_id) 
        REFERENCES service_requests(request_id) ON DELETE RESTRICT,
    CONSTRAINT fk_project_quote FOREIGN KEY (quote_id) 
        REFERENCES quotes(quote_id) ON DELETE RESTRICT,
    CONSTRAINT fk_project_citizen FOREIGN KEY (citizen_id) 
        REFERENCES users(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_project_provider FOREIGN KEY (provider_id) 
        REFERENCES provider_profiles(provider_id) ON DELETE RESTRICT,
    CONSTRAINT check_dates CHECK (
        (scheduled_end_date IS NULL OR scheduled_start_date IS NULL OR scheduled_end_date >= scheduled_start_date) AND
        (actual_completion_date IS NULL OR actual_start_date IS NULL OR actual_completion_date >= actual_start_date)
    ),
    CONSTRAINT check_warranty CHECK (
        (warranty_end_date IS NULL OR warranty_start_date IS NULL OR warranty_end_date >= warranty_start_date)
    )
);

-- Indexes for projects table
CREATE INDEX idx_project_request ON projects(request_id);
CREATE INDEX idx_project_quote ON projects(quote_id);
CREATE INDEX idx_project_citizen ON projects(citizen_id);
CREATE INDEX idx_project_provider ON projects(provider_id);
CREATE INDEX idx_project_status ON projects(status);
CREATE INDEX idx_project_provider_status ON projects(provider_id, status);
CREATE INDEX idx_project_citizen_status ON projects(citizen_id, status);
CREATE INDEX idx_project_completion ON projects(actual_completion_date DESC);
CREATE INDEX idx_project_payment ON projects(payment_status);

-- Comments
COMMENT ON TABLE projects IS 'Active and completed maintenance projects';

-- -----------------------------------------------------
-- Table: ratings
-- -----------------------------------------------------
CREATE TABLE ratings (
    rating_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    project_id UUID UNIQUE NOT NULL,
    citizen_id UUID NOT NULL,
    provider_id UUID NOT NULL,
    overall_rating INTEGER NOT NULL CHECK (overall_rating >= 1 AND overall_rating <= 5),
    quality_rating INTEGER NOT NULL CHECK (quality_rating >= 1 AND quality_rating <= 5),
    timeliness_rating INTEGER NOT NULL CHECK (timeliness_rating >= 1 AND timeliness_rating <= 5),
    professionalism_rating INTEGER NOT NULL CHECK (professionalism_rating >= 1 AND professionalism_rating <= 5),
    value_rating INTEGER NOT NULL CHECK (value_rating >= 1 AND value_rating <= 5),
    communication_rating INTEGER NOT NULL CHECK (communication_rating >= 1 AND communication_rating <= 5),
    review_title VARCHAR(255),
    review_text TEXT,
    would_recommend BOOLEAN,
    response_from_provider TEXT,
    response_at TIMESTAMP WITH TIME ZONE,
    is_verified BOOLEAN DEFAULT FALSE,
    is_featured BOOLEAN DEFAULT FALSE,
    helpful_count INTEGER DEFAULT 0 CHECK (helpful_count >= 0),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_rating_project FOREIGN KEY (project_id) 
        REFERENCES projects(project_id) ON DELETE CASCADE,
    CONSTRAINT fk_rating_citizen FOREIGN KEY (citizen_id) 
        REFERENCES users(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_rating_provider FOREIGN KEY (provider_id) 
        REFERENCES provider_profiles(provider_id) ON DELETE CASCADE
);

-- Indexes for ratings table
CREATE INDEX idx_rating_project ON ratings(project_id);
CREATE INDEX idx_rating_citizen ON ratings(citizen_id);
CREATE INDEX idx_rating_provider ON ratings(provider_id);
CREATE INDEX idx_rating_overall ON ratings(overall_rating DESC);
CREATE INDEX idx_rating_featured ON ratings(is_featured);
CREATE INDEX idx_rating_created ON ratings(created_at DESC);

-- Full-text search index
CREATE INDEX idx_rating_review_search ON ratings USING GIN (to_tsvector('english', COALESCE(review_text, '')));

-- Comments
COMMENT ON TABLE ratings IS 'Citizen ratings and reviews of completed projects';

-- -----------------------------------------------------
-- Table: audit_logs
-- -----------------------------------------------------
CREATE TABLE audit_logs (
    log_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID,
    action_type VARCHAR(50) NOT NULL,
    entity_type VARCHAR(50),
    entity_id UUID,
    action_description TEXT,
    old_values JSONB,
    new_values JSONB,
    ip_address VARCHAR(45),
    user_agent TEXT,
    request_url TEXT,
    response_status INTEGER,
    execution_time_ms INTEGER CHECK (execution_time_ms >= 0),
    error_message TEXT,
    session_id VARCHAR(255),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL
);

-- Indexes for audit_logs table
CREATE INDEX idx_audit_user ON audit_logs(user_id);
CREATE INDEX idx_audit_action ON audit_logs(action_type);
CREATE INDEX idx_audit_entity_type ON audit_logs(entity_type);
CREATE INDEX idx_audit_entity_id ON audit_logs(entity_id);
CREATE INDEX idx_audit_created ON audit_logs(created_at DESC);
CREATE INDEX idx_audit_user_created ON audit_logs(user_id, created_at DESC);
CREATE INDEX idx_audit_entity ON audit_logs(entity_type, entity_id);

-- Comments
COMMENT ON TABLE audit_logs IS 'System audit trail for security and compliance';

-- -----------------------------------------------------
-- Table: system_settings
-- -----------------------------------------------------
CREATE TABLE system_settings (
    setting_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    setting_key VARCHAR(100) UNIQUE NOT NULL,
    setting_value TEXT,
    data_type VARCHAR(50) DEFAULT 'string',
    category VARCHAR(100),
    description TEXT,
    is_public BOOLEAN DEFAULT FALSE,
    is_editable BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP NOT NULL
);

-- Indexes for system_settings table
CREATE INDEX idx_settings_key ON system_settings(setting_key);
CREATE INDEX idx_settings_category ON system_settings(category);
CREATE INDEX idx_settings_public ON system_settings(is_public);

-- Comments
COMMENT ON TABLE system_settings IS 'System-wide configuration settings';

-- =====================================================
-- FOREIGN KEY CONSTRAINTS (Added after table creation)
-- =====================================================

-- Add foreign key from service_requests to quotes (circular reference)
ALTER TABLE service_requests 
    ADD CONSTRAINT fk_request_selected_quote 
    FOREIGN KEY (selected_quote_id) 
    REFERENCES quotes(quote_id) ON DELETE SET NULL;

-- =====================================================
-- TRIGGERS FOR AUTOMATIC TIMESTAMP UPDATES
-- =====================================================

-- Create function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply trigger to all tables with updated_at column
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_provider_profiles_updated_at BEFORE UPDATE ON provider_profiles
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_portfolio_items_updated_at BEFORE UPDATE ON portfolio_items
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_service_requests_updated_at BEFORE UPDATE ON service_requests
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_quotes_updated_at BEFORE UPDATE ON quotes
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_projects_updated_at BEFORE UPDATE ON projects
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_ratings_updated_at BEFORE UPDATE ON ratings
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_system_settings_updated_at BEFORE UPDATE ON system_settings
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- FUNCTIONS AND STORED PROCEDURES
-- =====================================================

-- Function to calculate provider average rating
CREATE OR REPLACE FUNCTION calculate_provider_rating(p_provider_id UUID)
RETURNS DECIMAL(3,2) AS $$
DECLARE
    avg_rating DECIMAL(3,2);
BEGIN
    SELECT ROUND(AVG(overall_rating)::NUMERIC, 2)
    INTO avg_rating
    FROM ratings
    WHERE provider_id = p_provider_id;
    
    RETURN COALESCE(avg_rating, 0.00);
END;
$$ LANGUAGE plpgsql;

-- Function to update provider statistics
CREATE OR REPLACE FUNCTION update_provider_stats()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
        UPDATE provider_profiles
        SET 
            average_rating = calculate_provider_rating(NEW.provider_id),
            total_ratings = (SELECT COUNT(*) FROM ratings WHERE provider_id = NEW.provider_id),
            total_projects = (SELECT COUNT(*) FROM projects WHERE provider_id = NEW.provider_id),
            completion_rate = (
                SELECT ROUND(
                    (COUNT(*) FILTER (WHERE status = 'completed')::NUMERIC / 
                    NULLIF(COUNT(*), 0) * 100), 2
                )
                FROM projects 
                WHERE provider_id = NEW.provider_id
            )
        WHERE provider_id = NEW.provider_id;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger to update provider stats when rating is added/updated
CREATE TRIGGER update_provider_stats_on_rating
AFTER INSERT OR UPDATE ON ratings
FOR EACH ROW EXECUTE FUNCTION update_provider_stats();

-- Trigger to update provider stats when project status changes
CREATE TRIGGER update_provider_stats_on_project
AFTER INSERT OR UPDATE ON projects
FOR EACH ROW EXECUTE FUNCTION update_provider_stats();

-- Function to update quotes count
CREATE OR REPLACE FUNCTION update_request_quotes_count()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE service_requests
        SET quotes_count = quotes_count + 1
        WHERE request_id = NEW.request_id;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE service_requests
        SET quotes_count = GREATEST(quotes_count - 1, 0)
        WHERE request_id = OLD.request_id;
    END IF;
    
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Trigger to update quotes count
CREATE TRIGGER update_quotes_count_trigger
AFTER INSERT OR DELETE ON quotes
FOR EACH ROW EXECUTE FUNCTION update_request_quotes_count();

-- =====================================================
-- VIEWS FOR COMMON QUERIES
-- =====================================================

-- View: Active service requests with quote counts
CREATE OR REPLACE VIEW active_service_requests AS
SELECT 
    sr.request_id,
    sr.citizen_id,
    u.full_name AS citizen_name,
    sr.problem_title,
    sr.problem_category,
    sr.status,
    sr.quotes_count,
    sr.created_at,
    sr.expires_at
FROM service_requests sr
JOIN users u ON sr.citizen_id = u.user_id
WHERE sr.status IN ('open', 'quotes_received')
    AND (sr.expires_at IS NULL OR sr.expires_at > CURRENT_TIMESTAMP)
ORDER BY sr.created_at DESC;

-- View: Provider leaderboard
CREATE OR REPLACE VIEW provider_leaderboard AS
SELECT 
    pp.provider_id,
    pp.business_name,
    pp.provider_type,
    pp.average_rating,
    pp.total_projects,
    pp.total_ratings,
    pp.completion_rate,
    pp.is_verified,
    pp.is_featured
FROM provider_profiles pp
WHERE pp.is_verified = TRUE
ORDER BY pp.average_rating DESC, pp.total_projects DESC;

-- View: Project summary
CREATE OR REPLACE VIEW project_summary AS
SELECT 
    p.project_id,
    p.project_title,
    p.status,
    p.payment_status,
    uc.full_name AS citizen_name,
    pp.business_name AS provider_name,
    p.agreed_cost,
    p.actual_cost,
    p.scheduled_start_date,
    p.actual_completion_date,
    r.overall_rating,
    p.created_at
FROM projects p
JOIN users uc ON p.citizen_id = uc.user_id
JOIN provider_profiles pp ON p.provider_id = pp.provider_id
LEFT JOIN ratings r ON p.project_id = r.project_id
ORDER BY p.created_at DESC;

-- =====================================================
-- INITIAL DATA / SEED DATA
-- =====================================================

-- Insert default system settings
INSERT INTO system_settings (setting_key, setting_value, data_type, category, description, is_public, is_editable) VALUES
('platform_name', 'Smart Home Maintenance Platform', 'string', 'general', 'Platform display name', TRUE, TRUE),
('ai_model_version', 'v1.0.0', 'string', 'ai', 'Current AI diagnostic model version', FALSE, TRUE),
('quote_validity_days', '7', 'number', 'marketplace', 'Default quote validity period in days', TRUE, TRUE),
('max_quotes_per_request', '10', 'number', 'marketplace', 'Maximum quotes allowed per service request', TRUE, TRUE),
('min_rating_score', '1', 'number', 'ratings', 'Minimum rating score', TRUE, FALSE),
('max_rating_score', '5', 'number', 'ratings', 'Maximum rating score', TRUE, FALSE),
('platform_commission_rate', '15.00', 'number', 'payment', 'Platform commission percentage', FALSE, TRUE),
('maintenance_mode', 'false', 'boolean', 'system', 'Enable maintenance mode', FALSE, TRUE);

-- =====================================================
-- COMPLETION MESSAGE
-- =====================================================

DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Database setup completed successfully!';
    RAISE NOTICE 'Database: smart_home_maintenance';
    RAISE NOTICE 'Tables created: 10';
    RAISE NOTICE 'Indexes created: Multiple per table';
    RAISE NOTICE 'Triggers created: Auto-update timestamps';
    RAISE NOTICE 'Views created: 3 summary views';
    RAISE NOTICE '========================================';
END $$;

-- Display table sizes
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
