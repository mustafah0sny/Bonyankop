# Entity Relationship Diagram (ERD)
## Smart Home Maintenance & Public Infrastructure Platform

---

## 📋 Table of Contents
1. [ERD Overview](#erd-overview)
2. [Complete System ERD](#complete-system-erd)
3. [Core Entities ERD](#core-entities-erd)
4. [Service Marketplace ERD](#service-marketplace-erd)
5. [Detailed Entity Descriptions](#detailed-entity-descriptions)
6. [Relationship Cardinality](#relationship-cardinality)
7. [ERD Legend](#erd-legend)

---

## 🎯 ERD Overview

### Diagram Notation

**Entity Representation:**
```
┌─────────────────────┐
│   TABLE_NAME        │
├─────────────────────┤
│ PK: primary_key     │
│ FK: foreign_key     │
│     column_name     │
│     column_name     │
└─────────────────────┘
```

**Relationship Notation:**
- `─────` : One-to-One (1:1)
- `────<` : One-to-Many (1:N)
- `────<<` : Many-to-Many (M:N)
- `PK` : Primary Key
- `FK` : Foreign Key
- `*` : Required (NOT NULL)
- `○` : Optional (NULL)

---

## 🗺️ Complete System ERD

### High-Level System Architecture

```
                    ┌─────────────────────────────────────────────────────┐
                    │           SMART HOME MAINTENANCE PLATFORM           │
                    │                  DATABASE SCHEMA                    │
                    └─────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────┐
│                                                                                  │
│                          USER MANAGEMENT SUBSYSTEM                               │
│                                                                                  │
│  ┌─────────────────┐                    ┌──────────────────────┐               │
│  │     USERS       │                    │  PROVIDER_PROFILES   │               │
│  ├─────────────────┤                    ├──────────────────────┤               │
│  │ PK: user_id     │──────1:1──────────│ PK: provider_id      │               │
│  │  *  email       │                    │ FK: user_id          │               │
│  │  *  password    │                    │  *  business_name    │               │
│  │  *  role        │                    │     services_offered │               │
│  │  *  full_name   │                    │     average_rating   │               │
│  │     phone       │                    │     total_projects   │               │
│  └─────────────────┘                    └──────────────────────┘               │
│         │                                         │                              │
│         │                                         │                              │
└─────────┼─────────────────────────────────────────┼──────────────────────────────┘
          │                                         │
          │                                         │
          │                                         ▼
          │                              ┌──────────────────────┐
          │                              │  PORTFOLIO_ITEMS     │
          │                              ├──────────────────────┤
          │                              │ PK: portfolio_id     │
          │                              │ FK: provider_id      │
          │                              │  *  title            │
          │                              │     description      │
          │                              │     images (JSONB)   │
          │                              └──────────────────────┘
          │
          │
┌─────────┼─────────────────────────────────────────────────────────────────────┐
│         │                SERVICE MARKETPLACE SUBSYSTEM                         │
│         │                                                                      │
│         ▼                                                                      │
│  ┌─────────────────┐         ┌──────────────────────┐                        │
│  │   DIAGNOSTICS   │         │  SERVICE_REQUESTS    │                        │
│  ├─────────────────┤         ├──────────────────────┤                        │
│  │ PK: diagnostic_id│────<───│ PK: request_id       │                        │
│  │ FK: citizen_id  │         │ FK: diagnostic_id    │                        │
│  │  *  image_url   │         │ FK: citizen_id       │                        │
│  │  *  risk_level  │         │  *  problem_title    │                        │
│  │  *  category    │         │  *  status           │                        │
│  │     cause       │         │ FK: selected_quote_id│                        │
│  └─────────────────┘         └──────────────────────┘                        │
│                                        │                                       │
│                                        │                                       │
│                                        ▼                                       │
│                              ┌──────────────────────┐                         │
│                              │      QUOTES          │                         │
│                              ├──────────────────────┤                         │
│                              │ PK: quote_id         │                         │
│                              │ FK: request_id       │                         │
│                              │ FK: provider_id      │                         │
│                              │  *  estimated_cost   │                         │
│                              │     duration_days    │                         │
│                              │  *  status           │                         │
│                              └──────────────────────┘                         │
│                                        │                                       │
│                                        │                                       │
│                                        ▼                                       │
│                              ┌──────────────────────┐                         │
│                              │     PROJECTS         │                         │
│                              ├──────────────────────┤                         │
│                              │ PK: project_id       │                         │
│                              │ FK: request_id       │                         │
│                              │ FK: quote_id         │                         │
│                              │ FK: citizen_id       │                         │
│                              │ FK: provider_id      │                         │
│                              │  *  status           │                         │
│                              │     agreed_cost      │                         │
│                              │     actual_cost      │                         │
│                              └──────────────────────┘                         │
│                                        │                                       │
│                                        │                                       │
│                                        │ 1:1                                   │
│                                        ▼                                       │
│                              ┌──────────────────┐                             │
│                              │     RATINGS      │                             │
│                              ├──────────────────┤                             │
│                              │ PK: rating_id    │                             │
│                              │ FK: project_id   │                             │
│                              │ FK: citizen_id   │                             │
│                              │ FK: provider_id  │                             │
│                              │  *  overall (1-5)│                             │
│                              │     review_text  │                             │
│                              └──────────────────┘                             │
│                                                                               │
└───────────────────────────────────────────────────────────────────────────────┘


┌───────────────────────────────────────────────────────────────────────────────┐
│                                                                               │
│                      SYSTEM SUPPORT SUBSYSTEM                                 │
│                                                                               │
│  ┌─────────────────┐                                                         │
│  │     USERS       │                                                         │
│  └────────┬────────┘                                                         │
│           │                                                                   │
│           │                                                                   │
│           ├────────────────────────┐                                         │
│           ▼                        ▼                                         │
│  ┌──────────────────┐    ┌──────────────────┐                              │
│  │   AUDIT_LOGS     │    │ SYSTEM_SETTINGS  │                              │
│  ├──────────────────┤    ├──────────────────┤                              │
│  │ PK: log_id       │    │ PK: setting_id   │                              │
│  │ FK: user_id      │    │  *  setting_key  │                              │
│  │  *  action_type  │    │     setting_value│                              │
│  │     entity_type  │    │     category     │                              │
│  │     entity_id    │    │     is_public    │                              │
│  │     old_values   │    └──────────────────┘                              │
│  │     new_values   │                                                        │
│  │     ip_address   │                                                        │
│  │  *  created_at   │                                                        │
│  └──────────────────┘                                                        │
│                                                                               │
└───────────────────────────────────────────────────────────────────────────────┘
```

---

## 🏗️ Core Entities ERD

### User Management and Provider Profiles

```
┌────────────────────────────────────────────────────────────────────────┐
│                     CORE USER & PROVIDER ENTITIES                      │
└────────────────────────────────────────────────────────────────────────┘


                    ┌───────────────────────────────┐
                    │          USERS                │
                    ├───────────────────────────────┤
                    │ PK: user_id (UUID)            │
                    │  *  email (VARCHAR 255) UQ    │
                    │  *  password_hash (VARCHAR)   │
                    │  *  role (VARCHAR 50)         │
                    │     • citizen                 │
                    │     • engineer                │
                    │     • company                 │
                    │     • government              │
                    │     • admin                   │
                    │  *  full_name (VARCHAR 255)   │
                    │  ○  phone_number (VARCHAR 20) │
                    │  ○  profile_picture_url       │
                    │     is_verified (BOOLEAN)     │
                    │     is_active (BOOLEAN)       │
                    │     last_login_at (TIMESTAMP) │
                    │     created_at (TIMESTAMP)    │
                    │     updated_at (TIMESTAMP)    │
                    └───────────────┬───────────────┘
                                    │
                                    │ 1:1
                                    │ (Only for role = engineer/company)
                                    │
                                    ▼
                    ┌───────────────────────────────────────┐
                    │      PROVIDER_PROFILES                │
                    ├───────────────────────────────────────┤
                    │ PK: provider_id (UUID)                │
                    │ FK: user_id (UUID) UQ                 │
                    │  *  provider_type (VARCHAR 50)        │
                    │     • company                         │
                    │     • engineer                        │
                    │  *  business_name (VARCHAR 255)       │
                    │  ○  description (TEXT)                │
                    │  ○  services_offered (JSONB)          │
                    │     ["plumbing", "electrical", ...]   │
                    │  ○  certifications (JSONB)            │
                    │  ○  coverage_areas (JSONB)            │
                    │  ○  license_number (VARCHAR 100)      │
                    │  ○  years_of_experience (INTEGER)     │
                    │     average_rating (DECIMAL 3,2)      │
                    │     total_projects (INTEGER)          │
                    │     total_ratings (INTEGER)           │
                    │     completion_rate (DECIMAL 5,2)     │
                    │  ○  response_time_hours (DECIMAL 5,2) │
                    │     is_verified (BOOLEAN)             │
                    │     is_featured (BOOLEAN)             │
                    │     created_at (TIMESTAMP)            │
                    │     updated_at (TIMESTAMP)            │
                    └───────────────┬───────────────────────┘
                                    │
                                    │ 1:N
                                    │
                                    ▼
                    ┌───────────────────────────────┐
                    │     PORTFOLIO_ITEMS           │
                    ├───────────────────────────────┤
                    │ PK: portfolio_id (UUID)       │
                    │ FK: provider_id (UUID)        │
                    │  *  title (VARCHAR 255)       │
                    │  ○  description (TEXT)        │
                    │  ○  project_type (VARCHAR 100)│
                    │  ○  images (JSONB)            │
                    │     [{"url": "...", ...}]     │
                    │  ○  project_date (DATE)       │
                    │  ○  location (VARCHAR 255)    │
                    │     display_order (INTEGER)   │
                    │     is_featured (BOOLEAN)     │
                    │     created_at (TIMESTAMP)    │
                    │     updated_at (TIMESTAMP)    │
                    └───────────────────────────────┘


                    RELATIONSHIP DETAILS:
                    ═══════════════════════════════════════
                    
                    users → provider_profiles (1:1)
                    • One user can have ONE provider profile
                    • Only if role = 'engineer' OR 'company'
                    • ON DELETE CASCADE
                    
                    provider_profiles → portfolio_items (1:N)
                    • One provider can have MANY portfolio items
                    • Maximum 20 items per provider
                    • ON DELETE CASCADE
```

---

## 🛠️ Service Marketplace ERD

### Complete Service Request Flow

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    SERVICE MARKETPLACE FLOW                                │
└────────────────────────────────────────────────────────────────────────────┘


        ┌───────────────┐
        │     USERS     │
        │  (Citizen)    │
        └───────┬───────┘
                │
                │ 1:N
                │
                ▼
┌───────────────────────────────────┐
│         DIAGNOSTICS               │
├───────────────────────────────────┤
│ PK: diagnostic_id (UUID)          │
│ FK: citizen_id (UUID)             │
│  *  image_url (TEXT)              │
│  ○  image_metadata (JSONB)        │
│  *  risk_level (VARCHAR 50)       │
│     • low                         │
│     • medium                      │
│     • high                        │
│  *  problem_category (VARCHAR 100)│
│     • plumbing                    │
│     • electrical                  │
│     • structural                  │
│     • hvac                        │
│     • roofing                     │
│  ○  problem_subcategory           │
│  *  probable_cause (TEXT)         │
│  *  risk_prediction (TEXT)        │
│  *  recommended_action (TEXT)     │
│  *  ai_confidence_score (DEC 5,2) │
│  ○  ai_model_version (VARCHAR 50) │
│  ○  processing_time_ms (INTEGER)  │
│     is_diy_possible (BOOLEAN)     │
│  ○  estimated_cost_range (VARCHAR)│
│  ○  urgency_level (VARCHAR 50)    │
│     created_at (TIMESTAMP)        │
└───────────────┬───────────────────┘
                │
                │ 1:N (Optional)
                │
                ▼
┌───────────────────────────────────────┐
│       SERVICE_REQUESTS                │
├───────────────────────────────────────┤
│ PK: request_id (UUID)                 │
│ FK: diagnostic_id (UUID) NULL         │
│ FK: citizen_id (UUID)                 │
│  *  problem_title (VARCHAR 255)       │
│  *  problem_description (TEXT)        │
│  *  problem_category (VARCHAR 100)    │
│  ○  additional_images (JSONB)         │
│  ○  preferred_provider_type (VARCHAR) │
│     • company                         │
│     • engineer                        │
│     • any                             │
│  ○  preferred_service_date (DATE)     │
│  ○  property_type (VARCHAR 50)        │
│  ○  property_address (TEXT)           │
│  ○  contact_phone (VARCHAR 20)        │
│  *  status (VARCHAR 50)               │
│     • open                            │
│     • quotes_received                 │
│     • provider_selected               │
│     • cancelled                       │
│ FK: selected_quote_id (UUID) NULL     │
│     quotes_count (INTEGER)            │
│     views_count (INTEGER)             │
│  ○  expires_at (TIMESTAMP)            │
│     created_at (TIMESTAMP)            │
│     updated_at (TIMESTAMP)            │
└───────────────┬───────────────────────┘
                │
                │ 1:N
                │
                ▼
┌───────────────────────────────────────────┐
│            QUOTES                         │
├───────────────────────────────────────────┤
│ PK: quote_id (UUID)                       │
│ FK: request_id (UUID)                     │
│ FK: provider_id (UUID)                    │
│  *  estimated_cost (DECIMAL 10,2)         │
│  ○  cost_breakdown (JSONB)                │
│     {labor: {...}, materials: [...]}      │
│  ○  estimated_duration_days (INTEGER)     │
│  ○  technical_assessment (TEXT)           │
│  ○  proposed_solution (TEXT)              │
│     materials_included (BOOLEAN)          │
│  ○  warranty_period_months (INTEGER)      │
│  ○  terms_and_conditions (TEXT)           │
│     validity_period_days (INTEGER)        │
│  ○  attachments (JSONB)                   │
│  *  status (VARCHAR 50)                   │
│     • pending                             │
│     • accepted                            │
│     • rejected                            │
│     • expired                             │
│     • withdrawn                           │
│  ○  rejection_reason (TEXT)               │
│     submitted_at (TIMESTAMP)              │
│  *  expires_at (TIMESTAMP)                │
│  ○  accepted_at (TIMESTAMP)               │
│     updated_at (TIMESTAMP)                │
└───────────────┬───────────────────────────┘
                │
                │ 1:1 (When accepted)
                │
                ▼
┌───────────────────────────────────────────────┐
│              PROJECTS                         │
├───────────────────────────────────────────────┤
│ PK: project_id (UUID)                         │
│ FK: request_id (UUID) UQ                      │
│ FK: quote_id (UUID) UQ                        │
│ FK: citizen_id (UUID)                         │
│ FK: provider_id (UUID)                        │
│  *  project_title (VARCHAR 255)               │
│  ○  project_description (TEXT)                │
│  *  status (VARCHAR 50)                       │
│     • scheduled                               │
│     • in_progress                             │
│     • completed                               │
│     • cancelled                               │
│     • disputed                                │
│  ○  scheduled_start_date (DATE)               │
│  ○  actual_start_date (DATE)                  │
│  ○  scheduled_end_date (DATE)                 │
│  ○  actual_completion_date (DATE)             │
│  *  agreed_cost (DECIMAL 10,2)                │
│  ○  actual_cost (DECIMAL 10,2)                │
│  ○  cost_difference_reason (TEXT)             │
│     payment_status (VARCHAR 50)               │
│     • pending                                 │
│     • partial                                 │
│     • completed                               │
│     • refunded                                │
│  ○  work_notes (JSONB)                        │
│     [{timestamp, author, note, images}]       │
│  ○  before_images (JSONB)                     │
│  ○  during_images (JSONB)                     │
│  ○  after_images (JSONB)                      │
│  ○  technical_report_url (TEXT)               │
│  ○  completion_certificate_url (TEXT)         │
│  ○  warranty_start_date (DATE)                │
│  ○  warranty_end_date (DATE)                  │
│  ○  citizen_satisfaction (VARCHAR 50)         │
│     created_at (TIMESTAMP)                    │
│     updated_at (TIMESTAMP)                    │
└───────────────┬───────────────────────────────┘
                │
                │
                │
                │ 1:1
                │
                ▼
        ┌─────────────────────────────┐
        │        RATINGS              │
        ├─────────────────────────────┤
        │ PK: rating_id (UUID)        │
        │ FK: project_id (UUID) UQ    │
        │ FK: citizen_id (UUID)       │
        │ FK: provider_id (UUID)      │
        │  *  overall_rating (1-5)    │
        │  *  quality_rating (1-5)    │
        │  *  timeliness_rating (1-5) │
        │  *  professionalism (1-5)   │
        │  *  value_rating (1-5)      │
        │  *  communication (1-5)     │
        │  ○  review_title (VARCHAR)  │
        │  ○  review_text (TEXT)      │
        │  ○  would_recommend (BOOL)  │
        │  ○  response_from_provider  │
        │  ○  response_at (TIMESTAMP) │
        │     is_verified (BOOLEAN)   │
        │     is_featured (BOOLEAN)   │
        │     helpful_count (INTEGER) │
        │     created_at (TIMESTAMP)  │
        │     updated_at (TIMESTAMP)  │
        └─────────────────────────────┘


                    WORKFLOW SEQUENCE:
                    ═══════════════════════════════════════
                    
                    1. Citizen uploads image → DIAGNOSTICS
                    2. AI analyzes → Results stored
                    3. Citizen creates → SERVICE_REQUESTS
                    4. Providers submit → QUOTES (multiple)
                    5. Citizen accepts one → PROJECTS created
                    6. Work completed → RATINGS submitted
                    
                    BUSINESS RULES:
                    ═══════════════════════════════════════
                    
                    • One diagnostic can lead to multiple requests
                    • One request receives multiple quotes
                    • Only ONE quote can be accepted per request
                    • Accepted quote creates ONE project
                    • One project has ONE rating
                    • Provider can submit only ONE quote per request
```

---

## 💬 Communication & Notifications ERD

### System Communication Infrastructure

```
┌────────────────────────────────────────────────────────────────────────────┐
│                 COMMUNICATION & NOTIFICATION SUBSYSTEM                     │
└────────────────────────────────────────────────────────────────────────────┘


        ┌───────────────┐
        │     USERS     │
        └───────┬───────┘
                │
                │
        ┌───────┼───────────────────┐
        │       │                   │
        │ 1:N   │ 1:N               │ 1:N
        │       │                   │
        ▼       ▼                   ▼
┌──────────────────┐  ┌──────────────────────┐  ┌──────────────────┐
│  CHAT_MESSAGES   │  │   NOTIFICATIONS      │  │   AUDIT_LOGS     │
├──────────────────┤  ├──────────────────────┤  ├──────────────────┤
│ PK: message_id   │  │ PK: notification_id  │  │ PK: log_id       │
│    (UUID)        │  │    (UUID)            │  │    (UUID)        │
│                  │  │                      │  │                  │
│ FK: project_id   │  │ FK: user_id (UUID)   │  │ FK: user_id      │
│    (UUID)        │  │                      │  │    (UUID) NULL   │
│                  │  │  *  notification_type│  │                  │
│ FK: sender_id    │  │     (VARCHAR 50)     │  │  *  action_type  │
│    (UUID)        │  │     • new_quote      │  │     (VARCHAR 50) │
│                  │  │     • quote_accepted │  │     • user_login │
│ FK: receiver_id  │  │     • project_status │  │     • user_logout│
│    (UUID)        │  │     • new_message    │  │     • data_export│
│                  │  │     • report_update  │  │     • permission │
│  ○  message_text │  │     • rating_received│  │                  │
│     (TEXT)       │  │     • payment        │  │  ○  entity_type  │
│                  │  │     • announcement   │  │     (VARCHAR 50) │
│     message_type │  │                      │  │                  │
│     (VARCHAR 50) │  │  *  title (VARCHAR)  │  │  ○  entity_id    │
│     • text       │  │  *  message (TEXT)   │  │     (UUID)       │
│     • image      │  │                      │  │                  │
│     • file       │  │  ○  related_entity   │  │  ○  action_desc  │
│     • system     │  │     _type (VARCHAR)  │  │     (TEXT)       │
│                  │  │     • project        │  │                  │
│  ○  attachments  │  │     • quote          │  │  ○  old_values   │
│     (JSONB)      │  │     • report         │  │     (JSONB)      │
│     [{type, url, │  │     • message        │  │                  │
│       filename}] │  │                      │  │  ○  new_values   │
│                  │  │  ○  related_entity   │  │     (JSONB)      │
│     is_read      │  │     _id (UUID)       │  │                  │
│     (BOOLEAN)    │  │                      │  │  ○  ip_address   │
│                  │  │  ○  action_url (TEXT)│  │     (VARCHAR 45) │
│  ○  read_at      │  │                      │  │                  │
│     (TIMESTAMP)  │  │     priority         │  │  ○  user_agent   │
│                  │  │     (VARCHAR 50)     │  │     (TEXT)       │
│     is_deleted   │  │     • low            │  │                  │
│     _by_sender   │  │     • normal         │  │  ○  request_url  │
│     (BOOLEAN)    │  │     • high           │  │     (TEXT)       │
│                  │  │     • urgent         │  │                  │
│     is_deleted   │  │                      │  │  ○  response     │
│     _by_receiver │  │  ○  delivery_method  │  │     _status (INT)│
│     (BOOLEAN)    │  │     (JSONB)          │  │                  │
│                  │  │     {push, email,    │  │  ○  execution    │
│ FK: reply_to     │  │      sms}            │  │     _time_ms     │
│    _message_id   │  │                      │  │     (INTEGER)    │
│    (UUID) NULL   │  │     is_read          │  │                  │
│                  │  │     (BOOLEAN)        │  │  ○  error_message│
│     created_at   │  │                      │  │     (TEXT)       │
│     (TIMESTAMP)  │  │  ○  read_at          │  │                  │
└──────────────────┘  │     (TIMESTAMP)      │  │  ○  session_id   │
                      │                      │  │     (VARCHAR)    │
                      │     is_deleted       │  │                  │
                      │     (BOOLEAN)        │  │     created_at   │
                      │                      │  │     (TIMESTAMP)  │
                      │  ○  expires_at       │  └──────────────────┘
                      │     (TIMESTAMP)      │
                      │                      │
                      │     created_at       │
                      │     (TIMESTAMP)      │
                      └──────────────────────┘


                    CHAT MESSAGE FEATURES:
                    ═══════════════════════════════════════
                    
                    • Real-time via WebSocket
                    • Project-scoped conversations
                    • File attachments support
                    • Read receipts
                    • Threaded replies
                    • Soft delete (both parties)
                    
                    NOTIFICATION TRIGGERS:
                    ═══════════════════════════════════════
                    
                    • New quote received
                    • Quote accepted/rejected
                    • Project status change
                    • New chat message
                    • Public report update
                    • Rating received
                    • Payment processed
                    • System announcements
                    
                    AUDIT LOG PURPOSES:
                    ═══════════════════════════════════════
                    
                    • Security monitoring
                    • Compliance requirements
                    • Troubleshooting
                    • User activity tracking
                    • Data change history
                    • Immutable records
```

---

## 📊 Detailed Entity Descriptions

### Entity Summary Table

| Entity Name | Primary Purpose | Record Count (Est. Year 1) | Growth Rate |
|-------------|----------------|---------------------------|-------------|
| users | Store all user accounts | 50,000 | High |
| provider_profiles | Service provider details | 2,000 | Medium |
| portfolio_items | Provider work showcase | 10,000 | Medium |
| diagnostics | AI diagnostic results | 100,000 | High |
| service_requests | Service requests | 30,000 | High |
| quotes | Provider quotes | 90,000 | High |
| projects | Active/completed projects | 25,000 | High |
| ratings | Provider ratings | 20,000 | Medium |
| audit_logs | System activity logs | 2,000,000 | Very High |
| system_settings | Configuration | 100 | Very Low |

---

## 🔗 Relationship Cardinality

### Complete Relationship Matrix

```
┌─────────────────────────────────────────────────────────────────────────┐
│                     RELATIONSHIP CARDINALITY MATRIX                     │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  Parent Entity          Child Entity           Cardinality   Delete    │
│  ═════════════          ════════════           ═══════════   ══════    │
│                                                                         │
│  users                  provider_profiles      1:1           CASCADE   │
│  users                  diagnostics            1:N           SET NULL  │
│  users                  service_requests       1:N           SET NULL  │
│  users                  audit_logs             1:N           SET NULL  │
│                                                                         │
│  provider_profiles      portfolio_items        1:N           CASCADE   │
│  provider_profiles      quotes                 1:N           SET NULL  │
│  provider_profiles      projects               1:N           RESTRICT  │
│  provider_profiles      ratings                1:N           CASCADE   │
│                                                                         │
│  diagnostics            service_requests       1:N           SET NULL  │
│                                                                         │
│  service_requests       quotes                 1:N           CASCADE   │
│  service_requests       projects               1:1           RESTRICT  │
│                                                                         │
│  quotes                 projects               1:1           RESTRICT  │
│                                                                         │
│  projects               ratings                1:1           CASCADE   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘


CASCADE:    Delete child records when parent is deleted
SET NULL:   Set foreign key to NULL when parent is deleted
RESTRICT:   Prevent deletion of parent if children exist
NO ACTION:  Similar to RESTRICT, checked at transaction end
```

### Relationship Constraints

**One-to-One Relationships:**
```
users ←──────────→ provider_profiles
  (One user can have one provider profile, only if role = engineer/company)

service_requests ←──────────→ projects
  (One accepted request becomes one project)

quotes ←──────────→ projects
  (One accepted quote becomes one project)

projects ←──────────→ ratings
  (One completed project receives one rating)
```

**One-to-Many Relationships:**
```
users ←──────────< diagnostics
  (One citizen can have many diagnostics)

users ←──────────< service_requests
  (One citizen can create many requests)

provider_profiles ←──────────< quotes
  (One provider can submit many quotes)

service_requests ←──────────< quotes
  (One request can receive many quotes)
```

---

## 🎨 ERD Legend

### Symbols and Notation

```
┌─────────────────────────────────────────────────────────────────┐
│                         ERD LEGEND                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ENTITY BOX:                                                    │
│  ┌───────────────────┐                                         │
│  │   TABLE_NAME      │  ← Table name in UPPERCASE              │
│  ├───────────────────┤                                         │
│  │ PK: column_name   │  ← Primary Key                          │
│  │ FK: column_name   │  ← Foreign Key                          │
│  │  *  column_name   │  ← Required field (NOT NULL)            │
│  │  ○  column_name   │  ← Optional field (NULL)                │
│  │     column_name   │  ← Field (default behavior)             │
│  └───────────────────┘                                         │
│                                                                 │
│  RELATIONSHIPS:                                                 │
│                                                                 │
│  ──────────  One-to-One (1:1)                                  │
│              Example: users ←→ provider_profiles               │
│                                                                 │
│  ──────────< One-to-Many (1:N)                                 │
│              Example: users ←──< diagnostics                   │
│                                                                 │
│  ──────────<<  Many-to-Many (M:N)                              │
│                (Requires junction table)                        │
│                                                                 │
│  DATA TYPES:                                                    │
│                                                                 │
│  UUID         - Universally Unique Identifier                  │
│  VARCHAR(n)   - Variable character string, max length n        │
│  TEXT         - Unlimited text                                 │
│  INTEGER      - Whole number                                   │
│  DECIMAL(p,s) - Decimal number, p digits, s after decimal      │
│  BOOLEAN      - True/False                                     │
│  TIMESTAMP    - Date and time                                  │
│  DATE         - Date only                                      │
│  JSONB        - JSON Binary (PostgreSQL)                       │
│                                                                 │
│  CONSTRAINTS:                                                   │
│                                                                 │
│  PK           - Primary Key (unique, not null)                 │
│  FK           - Foreign Key (references another table)         │
│  UQ           - Unique constraint                              │
│  *            - NOT NULL constraint                            │
│  ○            - NULL allowed                                   │
│  CHECK        - Value validation                               │
│  DEFAULT      - Default value                                  │
│                                                                 │
│  INDEXES:                                                       │
│                                                                 │
│  [idx]        - Regular B-tree index                           │
│  [GIN]        - Generalized Inverted Index (JSONB, full-text)  │
│  [GiST]       - Generalized Search Tree (geospatial)           │
│  [UNIQUE]     - Unique index                                   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Color Coding (for visual tools)

```
┌─────────────────────────────────────────────────────────────────┐
│                    RECOMMENDED COLOR SCHEME                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  🔵 BLUE      - Core user entities (users, provider_profiles)   │
│  🟢 GREEN     - Service marketplace (diagnostics, requests)     │
│  🟡 YELLOW    - Projects and execution (projects, quotes)       │
│  🔴 RED       - Public reporting (public_reports, updates)      │
│  ⚫ GRAY      - System/audit (audit_logs, settings)             │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Tools for Creating Visual ERDs

### Recommended ERD Tools

**1. Online Tools (Free):**
- **draw.io (diagrams.net)** - Free, web-based, export to PNG/SVG
- **dbdiagram.io** - Database-specific, generates SQL
- **Lucidchart** - Professional diagrams, free tier available
- **ERDPlus** - Academic tool, simple interface

**2. Desktop Tools:**
- **MySQL Workbench** - Free, reverse engineer from database
- **pgAdmin** - PostgreSQL GUI, built-in ERD tool
- **DBeaver** - Free, supports multiple databases
- **DataGrip** - JetBrains, paid but powerful

**3. Code-Based Tools:**
- **Mermaid.js** - Markdown-based diagrams
- **PlantUML** - Text-based UML diagrams
- **dbml (Database Markup Language)** - Simple syntax

### Sample DBML Code

You can use this code in **dbdiagram.io** to generate a visual ERD:

```dbml
// Users and Authentication
Table users {
  user_id uuid [pk]
  email varchar(255) [unique, not null]
  password_hash varchar(255) [not null]
  role varchar(50) [not null]
  full_name varchar(255) [not null]
  phone_number varchar(20)
  profile_picture_url text
  is_verified boolean [default: false]
  is_active boolean [default: true]
  created_at timestamp [default: `now()`]
  updated_at timestamp [default: `now()`]
}

// Provider Profiles
Table provider_profiles {
  provider_id uuid [pk]
  user_id uuid [unique, not null, ref: - users.user_id]
  provider_type varchar(50) [not null]
  business_name varchar(255) [not null]
  description text
  services_offered jsonb
  average_rating decimal(3,2) [default: 0]
  total_projects integer [default: 0]
  is_verified boolean [default: false]
  created_at timestamp [default: `now()`]
}

// Portfolio Items
Table portfolio_items {
  portfolio_id uuid [pk]
  provider_id uuid [not null, ref: > provider_profiles.provider_id]
  title varchar(255) [not null]
  description text
  images jsonb
  project_date date
  created_at timestamp [default: `now()`]
}

// AI Diagnostics
Table diagnostics {
  diagnostic_id uuid [pk]
  citizen_id uuid [not null, ref: > users.user_id]
  image_url text [not null]
  risk_level varchar(50) [not null]
  problem_category varchar(100) [not null]
  probable_cause text [not null]
  ai_confidence_score decimal(5,2) [not null]
  created_at timestamp [default: `now()`]
}

// Service Requests
Table service_requests {
  request_id uuid [pk]
  diagnostic_id uuid [ref: > diagnostics.diagnostic_id]
  citizen_id uuid [not null, ref: > users.user_id]
  problem_title varchar(255) [not null]
  problem_description text [not null]
  status varchar(50) [not null, default: 'open']
  selected_quote_id uuid [ref: - quotes.quote_id]
  created_at timestamp [default: `now()`]
}

// Quotes
Table quotes {
  quote_id uuid [pk]
  request_id uuid [not null, ref: > service_requests.request_id]
  provider_id uuid [not null, ref: > provider_profiles.provider_id]
  estimated_cost decimal(10,2) [not null]
  status varchar(50) [not null, default: 'pending']
  submitted_at timestamp [default: `now()`]
}

// Projects
Table projects {
  project_id uuid [pk]
  request_id uuid [unique, not null, ref: - service_requests.request_id]
  quote_id uuid [unique, not null, ref: - quotes.quote_id]
  citizen_id uuid [not null, ref: > users.user_id]
  provider_id uuid [not null, ref: > provider_profiles.provider_id]
  status varchar(50) [not null, default: 'scheduled']
  agreed_cost decimal(10,2) [not null]
  created_at timestamp [default: `now()`]
}

// Ratings
Table ratings {
  rating_id uuid [pk]
  project_id uuid [unique, not null, ref: - projects.project_id]
  citizen_id uuid [not null, ref: > users.user_id]
  provider_id uuid [not null, ref: > provider_profiles.provider_id]
  overall_rating integer [not null]
  review_text text
  created_at timestamp [default: `now()`]
}

// Audit Logs
Table audit_logs {
  log_id uuid [pk]
  user_id uuid [ref: > users.user_id]
  action_type varchar(50) [not null]
  entity_type varchar(50)
  entity_id uuid
  created_at timestamp [default: `now()`]
}
```

### How to Use DBML Code:

1. Go to **https://dbdiagram.io**
2. Create a new diagram
3. Paste the DBML code above
4. The visual ERD will be generated automatically
5. Export as PNG, PDF, or SQL

---

## 📝 ERD Best Practices

### Design Principles

**1. Normalization:**
- Eliminate data redundancy
- Ensure data integrity
- Use appropriate normal forms (3NF minimum)

**2. Naming Conventions:**
- Use clear, descriptive names
- Consistent naming patterns
- Plural for tables, singular for columns
- Prefix foreign keys with table name

**3. Relationships:**
- Define all relationships explicitly
- Use appropriate cardinality
- Set proper cascade rules
- Document business rules

**4. Indexes:**
- Index all foreign keys
- Index frequently queried columns
- Avoid over-indexing
- Monitor index usage

**5. Documentation:**
- Document each entity's purpose
- Explain complex relationships
- Note business constraints
- Keep ERD updated with schema changes

---

## 🎓 Conclusion

This ERD documentation provides a complete visual representation of your database structure. The diagrams show:

✅ **10 Core Tables** with all columns and data types
✅ **All Relationships** with proper cardinality
✅ **Foreign Key Constraints** and cascade rules
✅ **Business Logic** embedded in the structure
✅ **Scalable Design** for future growth

### Next Steps:

1. **Review the ERD** with your team/advisor
2. **Generate Visual Diagrams** using dbdiagram.io or draw.io
3. **Validate Relationships** against business requirements
4. **Create Database Schema** from the ERD
5. **Implement in PostgreSQL** using migration scripts
6. **Create database views/queries** for generating reports dynamically

This ERD serves as the blueprint for your entire database implementation! 🎯

---

*Document Version: 3.0*  
*Last Updated: November 2025*  
*Total Entities: 10*  
*Total Relationships: 15+*  
*Notation: Crow's Foot / Chen Notation*

