-- ============================================================================
-- PROYECTO: GYM MANAGER
-- ARCHIVO: 01_schema_creation.sql
-- DESCRIPCIÓN: Script de creación completa del esquema de base de datos
-- VERSIÓN: 1.0
-- FECHA: 2024
-- SGBD: PostgreSQL 16.x
-- ============================================================================
-- 
-- INSTRUCCIONES DE USO:
-- 1. Conectarse a la base de datos gym_manager_db en pgAdmin
-- 2. Abrir Query Tool (clic derecho en gym_manager_db → Query Tool)
-- 3. Copiar y pegar este script completo
-- 4. Ejecutar con F5
--
-- NOTA: El script está diseñado para ejecutarse en orden. Si necesitas
-- ejecutar secciones individuales, respeta el orden de dependencias.
-- ============================================================================

-- ============================================================================
-- SECCIÓN 0: CONFIGURACIÓN INICIAL
-- ============================================================================

-- Establecer el esquema de trabajo
SET search_path TO gym, public;

-- Eliminar tablas existentes si se requiere reinstalación (CUIDADO EN PRODUCCIÓN)
-- Descomentar solo si necesitas reiniciar desde cero
/*
DROP TABLE IF EXISTS gym.ClassEnrollments CASCADE;
DROP TABLE IF EXISTS gym.ClassSchedules CASCADE;
DROP TABLE IF EXISTS gym.ClassTypes CASCADE;
DROP TABLE IF EXISTS gym.SaleItems CASCADE;
DROP TABLE IF EXISTS gym.Sales CASCADE;
DROP TABLE IF EXISTS gym.Payments CASCADE;
DROP TABLE IF EXISTS gym.PaymentMethods CASCADE;
DROP TABLE IF EXISTS gym.InventoryStock CASCADE;
DROP TABLE IF EXISTS gym.Products CASCADE;
DROP TABLE IF EXISTS gym.ProductCategories CASCADE;
DROP TABLE IF EXISTS gym.Attendances CASCADE;
DROP TABLE IF EXISTS gym.Memberships CASCADE;
DROP TABLE IF EXISTS gym.MembershipPlans CASCADE;
DROP TABLE IF EXISTS gym.Members CASCADE;
DROP TABLE IF EXISTS gym.UserRoles CASCADE;
DROP TABLE IF EXISTS gym.Users CASCADE;
DROP TABLE IF EXISTS gym.Roles CASCADE;
DROP TABLE IF EXISTS gym.BranchSettings CASCADE;
DROP TABLE IF EXISTS gym.Branches CASCADE;
DROP TABLE IF EXISTS gym.Licenses CASCADE;
*/

-- ============================================================================
-- SECCIÓN 1: EXTENSIONES REQUERIDAS
-- ============================================================================

-- Habilitar extensión para generación de UUIDs
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================================
-- SECCIÓN 2: TIPOS ENUMERADOS (ENUMS)
-- ============================================================================

-- Tipo de licencia
DO $$ BEGIN
    CREATE TYPE license_type_enum AS ENUM ('TRIAL', 'STANDARD', 'ENTERPRISE');
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- Estado de membresía
DO $$ BEGIN
    CREATE TYPE membership_status_enum AS ENUM ('ACTIVE', 'EXPIRED', 'FROZEN', 'CANCELLED');
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- Estado de pago
DO $$ BEGIN
    CREATE TYPE payment_status_enum AS ENUM ('COMPLETED', 'PENDING', 'REFUNDED', 'FAILED');
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- Estado de venta
DO $$ BEGIN
    CREATE TYPE sale_status_enum AS ENUM ('COMPLETED', 'PENDING', 'CANCELLED', 'REFUNDED');
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- Estado de inscripción a clase
DO $$ BEGIN
    CREATE TYPE enrollment_status_enum AS ENUM ('ENROLLED', 'ATTENDED', 'NO_SHOW', 'CANCELLED');
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- Método de check-in
DO $$ BEGIN
    CREATE TYPE checkin_method_enum AS ENUM ('CARD', 'QR', 'BIOMETRIC', 'MANUAL');
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- Género
DO $$ BEGIN
    CREATE TYPE gender_enum AS ENUM ('M', 'F', 'O');
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- ============================================================================
-- SECCIÓN 3: TABLAS DE LICENCIAMIENTO Y SEGURIDAD
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: Licenses
-- Descripción: Almacena información de licencias vinculadas a dongles/USB
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Licenses (
    license_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    license_key VARCHAR(256) NOT NULL,
    hardware_id VARCHAR(128) NOT NULL,
    license_type license_type_enum NOT NULL DEFAULT 'TRIAL',
    max_branches INTEGER NOT NULL DEFAULT 1,
    max_users INTEGER NOT NULL DEFAULT 5,
    issued_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    expires_at TIMESTAMP WITH TIME ZONE NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    last_validation TIMESTAMP WITH TIME ZONE NULL,
    validation_count INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT uq_licenses_license_key UNIQUE (license_key),
    CONSTRAINT uq_licenses_hardware_id UNIQUE (hardware_id),
    CONSTRAINT chk_licenses_max_branches CHECK (max_branches > 0),
    CONSTRAINT chk_licenses_max_users CHECK (max_users > 0),
    CONSTRAINT chk_licenses_validation_count CHECK (validation_count >= 0)
);

-- Índices para Licenses
CREATE INDEX IF NOT EXISTS idx_licenses_hardware_id ON gym.Licenses(hardware_id);
CREATE INDEX IF NOT EXISTS idx_licenses_is_active ON gym.Licenses(is_active) WHERE is_active = TRUE;
CREATE INDEX IF NOT EXISTS idx_licenses_expires_at ON gym.Licenses(expires_at) WHERE expires_at IS NOT NULL;

-- Comentarios
COMMENT ON TABLE gym.Licenses IS 'Almacena las licencias del software vinculadas a dispositivos dongle/USB';
COMMENT ON COLUMN gym.Licenses.license_key IS 'Clave de licencia cifrada con AES-256';
COMMENT ON COLUMN gym.Licenses.hardware_id IS 'Identificador único del dongle/USB físico';

-- ============================================================================
-- SECCIÓN 4: TABLAS DE ESTRUCTURA ORGANIZACIONAL
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: Branches
-- Descripción: Representa cada sucursal del gimnasio
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Branches (
    branch_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    license_id UUID NOT NULL,
    branch_code VARCHAR(10) NOT NULL,
    branch_name VARCHAR(100) NOT NULL,
    address VARCHAR(255) NULL,
    city VARCHAR(100) NULL,
    state VARCHAR(100) NULL,
    postal_code VARCHAR(20) NULL,
    country VARCHAR(50) DEFAULT 'México',
    phone VARCHAR(20) NULL,
    email VARCHAR(100) NULL,
    is_headquarters BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NULL,
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    updated_by UUID NULL,
    deleted_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT uq_branches_branch_code UNIQUE (branch_code),
    CONSTRAINT fk_branches_license FOREIGN KEY (license_id) 
        REFERENCES gym.Licenses(license_id) ON DELETE RESTRICT
);

-- Índices para Branches
CREATE INDEX IF NOT EXISTS idx_branches_license_id ON gym.Branches(license_id);
CREATE INDEX IF NOT EXISTS idx_branches_is_active ON gym.Branches(is_active) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_branches_city ON gym.Branches(city);

-- Comentarios
COMMENT ON TABLE gym.Branches IS 'Sucursales del gimnasio - núcleo de la arquitectura multi-sucursal';
COMMENT ON COLUMN gym.Branches.branch_code IS 'Código único corto de la sucursal (ej: GYM-001)';
COMMENT ON COLUMN gym.Branches.deleted_at IS 'Soft delete - NULL significa registro activo';

-- -----------------------------------------------------------------------------
-- Tabla: BranchSettings
-- Descripción: Configuración visual y operativa por sucursal
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.BranchSettings (
    setting_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NOT NULL,
    primary_color VARCHAR(7) DEFAULT '#1E40AF',
    secondary_color VARCHAR(7) DEFAULT '#3B82F6',
    accent_color VARCHAR(7) DEFAULT '#10B981',
    logo_path VARCHAR(500) NULL,
    logo_small_path VARCHAR(500) NULL,
    favicon_path VARCHAR(500) NULL,
    business_name VARCHAR(150) NULL,
    tax_id VARCHAR(50) NULL,
    receipt_header TEXT NULL,
    receipt_footer TEXT NULL,
    timezone VARCHAR(50) DEFAULT 'America/Mexico_City',
    currency_code VARCHAR(3) DEFAULT 'MXN',
    currency_symbol VARCHAR(5) DEFAULT '$',
    date_format VARCHAR(20) DEFAULT 'dd/MM/yyyy',
    time_format VARCHAR(20) DEFAULT 'HH:mm',
    opening_time TIME DEFAULT '06:00',
    closing_time TIME DEFAULT '22:00',
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT uq_branchsettings_branch_id UNIQUE (branch_id),
    CONSTRAINT fk_branchsettings_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE CASCADE,
    CONSTRAINT chk_branchsettings_primary_color CHECK (primary_color ~ '^#[0-9A-Fa-f]{6}$'),
    CONSTRAINT chk_branchsettings_secondary_color CHECK (secondary_color ~ '^#[0-9A-Fa-f]{6}$'),
    CONSTRAINT chk_branchsettings_accent_color CHECK (accent_color ~ '^#[0-9A-Fa-f]{6}$')
);

-- Comentarios
COMMENT ON TABLE gym.BranchSettings IS 'Configuración de personalización de marca por sucursal';
COMMENT ON COLUMN gym.BranchSettings.primary_color IS 'Color primario en formato hexadecimal (#RRGGBB)';

-- ============================================================================
-- SECCIÓN 5: TABLAS DE USUARIOS Y ROLES
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: Roles
-- Descripción: Catálogo de roles del sistema
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Roles (
    role_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    role_name VARCHAR(50) NOT NULL,
    role_code VARCHAR(20) NOT NULL,
    description VARCHAR(255) NULL,
    permission_level INTEGER DEFAULT 0,
    is_system_role BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    -- Restricciones
    CONSTRAINT uq_roles_role_name UNIQUE (role_name),
    CONSTRAINT uq_roles_role_code UNIQUE (role_code)
);

-- Comentarios
COMMENT ON TABLE gym.Roles IS 'Catálogo de roles del sistema (ADMIN, STAFF, TRAINER, CLIENT)';
COMMENT ON COLUMN gym.Roles.is_system_role IS 'Indica si es un rol del sistema que no puede eliminarse';

-- -----------------------------------------------------------------------------
-- Tabla: Users
-- Descripción: Usuarios del sistema (personal administrativo y entrenadores)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Users (
    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NOT NULL,
    employee_code VARCHAR(20) NULL,
    email VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    password_salt VARCHAR(100) NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    phone VARCHAR(20) NULL,
    avatar_path VARCHAR(500) NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    must_change_password BOOLEAN DEFAULT FALSE,
    last_login_at TIMESTAMP WITH TIME ZONE NULL,
    last_login_ip VARCHAR(45) NULL,
    failed_login_attempts INTEGER DEFAULT 0,
    locked_until TIMESTAMP WITH TIME ZONE NULL,
    password_changed_at TIMESTAMP WITH TIME ZONE NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NULL,
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    updated_by UUID NULL,
    deleted_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT uq_users_email UNIQUE (email),
    CONSTRAINT uq_users_employee_code UNIQUE (employee_code),
    CONSTRAINT fk_users_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE RESTRICT,
    CONSTRAINT chk_users_failed_attempts CHECK (failed_login_attempts >= 0)
);

-- Índices para Users
CREATE INDEX IF NOT EXISTS idx_users_email ON gym.Users(email);
CREATE INDEX IF NOT EXISTS idx_users_branch_id ON gym.Users(branch_id);
CREATE INDEX IF NOT EXISTS idx_users_is_active ON gym.Users(is_active) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_users_employee_code ON gym.Users(employee_code) WHERE employee_code IS NOT NULL;

-- Agregar FK de auditoría después de crear la tabla
ALTER TABLE gym.Users 
    ADD CONSTRAINT fk_users_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL;
ALTER TABLE gym.Users 
    ADD CONSTRAINT fk_users_updated_by FOREIGN KEY (updated_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL;

-- Agregar FK de auditoría a Branches
ALTER TABLE gym.Branches 
    ADD CONSTRAINT fk_branches_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL;
ALTER TABLE gym.Branches 
    ADD CONSTRAINT fk_branches_updated_by FOREIGN KEY (updated_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL;

-- Comentarios
COMMENT ON TABLE gym.Users IS 'Usuarios del sistema incluyendo administradores, staff y entrenadores';
COMMENT ON COLUMN gym.Users.password_hash IS 'Contraseña hasheada con bcrypt';
COMMENT ON COLUMN gym.Users.deleted_at IS 'Soft delete - NULL significa registro activo';

-- -----------------------------------------------------------------------------
-- Tabla: UserRoles
-- Descripción: Relación N:M entre usuarios y roles
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.UserRoles (
    user_role_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    role_id UUID NOT NULL,
    branch_id UUID NULL,
    assigned_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    assigned_by UUID NULL,
    expires_at TIMESTAMP WITH TIME ZONE NULL,
    is_active BOOLEAN DEFAULT TRUE,
    
    -- Restricciones
    CONSTRAINT uq_userroles_user_role_branch UNIQUE (user_id, role_id, branch_id),
    CONSTRAINT fk_userroles_user FOREIGN KEY (user_id) 
        REFERENCES gym.Users(user_id) ON DELETE CASCADE,
    CONSTRAINT fk_userroles_role FOREIGN KEY (role_id) 
        REFERENCES gym.Roles(role_id) ON DELETE RESTRICT,
    CONSTRAINT fk_userroles_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE CASCADE,
    CONSTRAINT fk_userroles_assigned_by FOREIGN KEY (assigned_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL
);

-- Índices para UserRoles
CREATE INDEX IF NOT EXISTS idx_userroles_user_id ON gym.UserRoles(user_id);
CREATE INDEX IF NOT EXISTS idx_userroles_role_id ON gym.UserRoles(role_id);
CREATE INDEX IF NOT EXISTS idx_userroles_branch_id ON gym.UserRoles(branch_id);
CREATE INDEX IF NOT EXISTS idx_userroles_is_active ON gym.UserRoles(is_active) WHERE is_active = TRUE;

-- Comentarios
COMMENT ON TABLE gym.UserRoles IS 'Asignación de roles a usuarios - un usuario puede tener múltiples roles';
COMMENT ON COLUMN gym.UserRoles.branch_id IS 'Sucursal específica para el rol - NULL significa todas las sucursales';

-- ============================================================================
-- SECCIÓN 6: TABLAS DE GESTIÓN DE MIEMBROS
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: Members
-- Descripción: Clientes/Miembros del gimnasio
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Members (
    member_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NOT NULL,
    member_code VARCHAR(20) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) NULL,
    phone VARCHAR(20) NULL,
    mobile_phone VARCHAR(20) NULL,
    birth_date DATE NULL,
    gender gender_enum NULL,
    address VARCHAR(255) NULL,
    city VARCHAR(100) NULL,
    state VARCHAR(100) NULL,
    postal_code VARCHAR(20) NULL,
    emergency_contact_name VARCHAR(100) NULL,
    emergency_contact_phone VARCHAR(20) NULL,
    emergency_contact_relationship VARCHAR(50) NULL,
    photo_path VARCHAR(500) NULL,
    id_document_type VARCHAR(20) NULL,
    id_document_number VARCHAR(50) NULL,
    medical_notes TEXT NULL,
    notes TEXT NULL,
    referred_by_member_id UUID NULL,
    registration_date DATE NOT NULL DEFAULT CURRENT_DATE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NULL,
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    updated_by UUID NULL,
    deleted_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT uq_members_member_code UNIQUE (member_code),
    CONSTRAINT fk_members_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE RESTRICT,
    CONSTRAINT fk_members_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_members_updated_by FOREIGN KEY (updated_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_members_referred_by FOREIGN KEY (referred_by_member_id) 
        REFERENCES gym.Members(member_id) ON DELETE SET NULL
);

-- Índices para Members
CREATE INDEX IF NOT EXISTS idx_members_branch_id ON gym.Members(branch_id);
CREATE INDEX IF NOT EXISTS idx_members_member_code ON gym.Members(member_code);
CREATE INDEX IF NOT EXISTS idx_members_email ON gym.Members(email) WHERE email IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_members_phone ON gym.Members(phone) WHERE phone IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_members_name ON gym.Members(last_name, first_name);
CREATE INDEX IF NOT EXISTS idx_members_is_active ON gym.Members(is_active) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_members_registration_date ON gym.Members(registration_date);

-- Comentarios
COMMENT ON TABLE gym.Members IS 'Clientes/Miembros del gimnasio';
COMMENT ON COLUMN gym.Members.member_code IS 'Código único del miembro para identificación (tarjeta/QR)';
COMMENT ON COLUMN gym.Members.referred_by_member_id IS 'Programa de referidos - miembro que lo recomendó';

-- -----------------------------------------------------------------------------
-- Tabla: MembershipPlans
-- Descripción: Catálogo de planes/tipos de membresía
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.MembershipPlans (
    plan_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NULL,
    plan_name VARCHAR(100) NOT NULL,
    plan_code VARCHAR(20) NOT NULL,
    description TEXT NULL,
    duration_days INTEGER NOT NULL,
    duration_description VARCHAR(50) NULL,
    price DECIMAL(10,2) NOT NULL,
    enrollment_fee DECIMAL(10,2) DEFAULT 0,
    includes_classes BOOLEAN DEFAULT FALSE,
    max_classes_per_week INTEGER NULL,
    includes_trainer BOOLEAN DEFAULT FALSE,
    trainer_sessions_included INTEGER NULL,
    includes_locker BOOLEAN DEFAULT FALSE,
    includes_towel BOOLEAN DEFAULT FALSE,
    includes_parking BOOLEAN DEFAULT FALSE,
    max_freezing_days INTEGER DEFAULT 0,
    can_transfer BOOLEAN DEFAULT FALSE,
    can_share BOOLEAN DEFAULT FALSE,
    max_shared_users INTEGER NULL,
    valid_from DATE NULL,
    valid_until DATE NULL,
    sort_order INTEGER DEFAULT 0,
    is_featured BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NULL,
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    deleted_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT uq_membershipplans_plan_code UNIQUE (plan_code),
    CONSTRAINT fk_membershipplans_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE CASCADE,
    CONSTRAINT fk_membershipplans_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_membershipplans_duration CHECK (duration_days > 0),
    CONSTRAINT chk_membershipplans_price CHECK (price >= 0),
    CONSTRAINT chk_membershipplans_freezing CHECK (max_freezing_days >= 0)
);

-- Índices para MembershipPlans
CREATE INDEX IF NOT EXISTS idx_membershipplans_branch_id ON gym.MembershipPlans(branch_id);
CREATE INDEX IF NOT EXISTS idx_membershipplans_is_active ON gym.MembershipPlans(is_active) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_membershipplans_sort_order ON gym.MembershipPlans(sort_order);

-- Comentarios
COMMENT ON TABLE gym.MembershipPlans IS 'Catálogo de planes de membresía disponibles';
COMMENT ON COLUMN gym.MembershipPlans.branch_id IS 'NULL significa disponible en todas las sucursales';
COMMENT ON COLUMN gym.MembershipPlans.duration_days IS 'Duración del plan en días';

-- -----------------------------------------------------------------------------
-- Tabla: Memberships
-- Descripción: Membresías activas/históricas de los miembros
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Memberships (
    membership_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    member_id UUID NOT NULL,
    plan_id UUID NOT NULL,
    branch_id UUID NOT NULL,
    membership_number VARCHAR(30) NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    original_end_date DATE NOT NULL,
    price_paid DECIMAL(10,2) NOT NULL,
    discount_applied DECIMAL(10,2) DEFAULT 0,
    discount_reason VARCHAR(255) NULL,
    status membership_status_enum NOT NULL DEFAULT 'ACTIVE',
    frozen_at TIMESTAMP WITH TIME ZONE NULL,
    frozen_until DATE NULL,
    frozen_days_used INTEGER DEFAULT 0,
    frozen_reason VARCHAR(255) NULL,
    cancelled_at TIMESTAMP WITH TIME ZONE NULL,
    cancellation_reason VARCHAR(255) NULL,
    auto_renew BOOLEAN DEFAULT FALSE,
    notes TEXT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    updated_by UUID NULL,
    
    -- Restricciones
    CONSTRAINT uq_memberships_number UNIQUE (membership_number),
    CONSTRAINT fk_memberships_member FOREIGN KEY (member_id) 
        REFERENCES gym.Members(member_id) ON DELETE RESTRICT,
    CONSTRAINT fk_memberships_plan FOREIGN KEY (plan_id) 
        REFERENCES gym.MembershipPlans(plan_id) ON DELETE RESTRICT,
    CONSTRAINT fk_memberships_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE RESTRICT,
    CONSTRAINT fk_memberships_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE RESTRICT,
    CONSTRAINT fk_memberships_updated_by FOREIGN KEY (updated_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_memberships_dates CHECK (end_date >= start_date),
    CONSTRAINT chk_memberships_price CHECK (price_paid >= 0),
    CONSTRAINT chk_memberships_frozen_days CHECK (frozen_days_used >= 0)
);

-- Índices para Memberships
CREATE INDEX IF NOT EXISTS idx_memberships_member_id ON gym.Memberships(member_id);
CREATE INDEX IF NOT EXISTS idx_memberships_plan_id ON gym.Memberships(plan_id);
CREATE INDEX IF NOT EXISTS idx_memberships_branch_id ON gym.Memberships(branch_id);
CREATE INDEX IF NOT EXISTS idx_memberships_status ON gym.Memberships(status);
CREATE INDEX IF NOT EXISTS idx_memberships_dates ON gym.Memberships(start_date, end_date);
CREATE INDEX IF NOT EXISTS idx_memberships_end_date ON gym.Memberships(end_date) WHERE status = 'ACTIVE';

-- Comentarios
COMMENT ON TABLE gym.Memberships IS 'Registro de membresías de cada miembro (activas e históricas)';
COMMENT ON COLUMN gym.Memberships.original_end_date IS 'Fecha original antes de congelamientos';
COMMENT ON COLUMN gym.Memberships.frozen_days_used IS 'Días de congelamiento utilizados';

-- -----------------------------------------------------------------------------
-- Tabla: Attendances
-- Descripción: Registro de asistencias (check-in/check-out)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Attendances (
    attendance_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    member_id UUID NOT NULL,
    branch_id UUID NOT NULL,
    membership_id UUID NULL,
    check_in_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    check_out_at TIMESTAMP WITH TIME ZONE NULL,
    check_in_method checkin_method_enum DEFAULT 'MANUAL',
    check_in_device VARCHAR(50) NULL,
    registered_by UUID NULL,
    duration_minutes INTEGER NULL,
    is_guest BOOLEAN DEFAULT FALSE,
    guest_name VARCHAR(100) NULL,
    guest_invited_by UUID NULL,
    notes VARCHAR(255) NULL,
    
    -- Restricciones
    CONSTRAINT fk_attendances_member FOREIGN KEY (member_id) 
        REFERENCES gym.Members(member_id) ON DELETE RESTRICT,
    CONSTRAINT fk_attendances_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE RESTRICT,
    CONSTRAINT fk_attendances_membership FOREIGN KEY (membership_id) 
        REFERENCES gym.Memberships(membership_id) ON DELETE SET NULL,
    CONSTRAINT fk_attendances_registered_by FOREIGN KEY (registered_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_attendances_guest_invited_by FOREIGN KEY (guest_invited_by) 
        REFERENCES gym.Members(member_id) ON DELETE SET NULL,
    CONSTRAINT chk_attendances_checkout CHECK (check_out_at IS NULL OR check_out_at >= check_in_at)
);

-- Índices para Attendances
CREATE INDEX IF NOT EXISTS idx_attendances_member_id ON gym.Attendances(member_id);
CREATE INDEX IF NOT EXISTS idx_attendances_branch_id ON gym.Attendances(branch_id);
CREATE INDEX IF NOT EXISTS idx_attendances_membership_id ON gym.Attendances(membership_id);
CREATE INDEX IF NOT EXISTS idx_attendances_check_in_at ON gym.Attendances(check_in_at);
CREATE INDEX IF NOT EXISTS idx_attendances_branch_date ON gym.Attendances(branch_id, check_in_at);
CREATE INDEX IF NOT EXISTS idx_attendances_date ON gym.Attendances(DATE(check_in_at));

-- Comentarios
COMMENT ON TABLE gym.Attendances IS 'Registro de entradas y salidas de miembros al gimnasio';
COMMENT ON COLUMN gym.Attendances.check_in_method IS 'Método usado para el check-in: CARD, QR, BIOMETRIC, MANUAL';
COMMENT ON COLUMN gym.Attendances.is_guest IS 'Indica si es un invitado (visita de día)';

-- ============================================================================
-- SECCIÓN 7: TABLAS DE PAGOS
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: PaymentMethods
-- Descripción: Catálogo de métodos de pago
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.PaymentMethods (
    payment_method_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    method_name VARCHAR(50) NOT NULL,
    method_code VARCHAR(20) NOT NULL,
    description VARCHAR(255) NULL,
    requires_reference BOOLEAN DEFAULT FALSE,
    icon_name VARCHAR(50) NULL,
    sort_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    -- Restricciones
    CONSTRAINT uq_paymentmethods_method_name UNIQUE (method_name),
    CONSTRAINT uq_paymentmethods_method_code UNIQUE (method_code)
);

-- Comentarios
COMMENT ON TABLE gym.PaymentMethods IS 'Catálogo de métodos de pago aceptados';
COMMENT ON COLUMN gym.PaymentMethods.requires_reference IS 'Indica si requiere número de referencia (tarjeta, transferencia)';

-- -----------------------------------------------------------------------------
-- Tabla: Payments
-- Descripción: Registro de todos los pagos realizados
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Payments (
    payment_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NOT NULL,
    member_id UUID NULL,
    membership_id UUID NULL,
    sale_id UUID NULL,
    payment_method_id UUID NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    currency_code VARCHAR(3) DEFAULT 'MXN',
    exchange_rate DECIMAL(10,4) DEFAULT 1.0000,
    reference_number VARCHAR(100) NULL,
    authorization_code VARCHAR(50) NULL,
    payment_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    status payment_status_enum DEFAULT 'COMPLETED',
    refund_reason VARCHAR(255) NULL,
    refunded_at TIMESTAMP WITH TIME ZONE NULL,
    refunded_by UUID NULL,
    receipt_number VARCHAR(50) NULL,
    notes TEXT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    
    -- Restricciones
    CONSTRAINT fk_payments_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE RESTRICT,
    CONSTRAINT fk_payments_member FOREIGN KEY (member_id) 
        REFERENCES gym.Members(member_id) ON DELETE SET NULL,
    CONSTRAINT fk_payments_membership FOREIGN KEY (membership_id) 
        REFERENCES gym.Memberships(membership_id) ON DELETE SET NULL,
    CONSTRAINT fk_payments_payment_method FOREIGN KEY (payment_method_id) 
        REFERENCES gym.PaymentMethods(payment_method_id) ON DELETE RESTRICT,
    CONSTRAINT fk_payments_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE RESTRICT,
    CONSTRAINT fk_payments_refunded_by FOREIGN KEY (refunded_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_payments_amount CHECK (amount > 0)
);

-- Índices para Payments
CREATE INDEX IF NOT EXISTS idx_payments_branch_id ON gym.Payments(branch_id);
CREATE INDEX IF NOT EXISTS idx_payments_member_id ON gym.Payments(member_id);
CREATE INDEX IF NOT EXISTS idx_payments_membership_id ON gym.Payments(membership_id);
CREATE INDEX IF NOT EXISTS idx_payments_sale_id ON gym.Payments(sale_id);
CREATE INDEX IF NOT EXISTS idx_payments_payment_date ON gym.Payments(payment_date);
CREATE INDEX IF NOT EXISTS idx_payments_status ON gym.Payments(status);
CREATE INDEX IF NOT EXISTS idx_payments_receipt_number ON gym.Payments(receipt_number) WHERE receipt_number IS NOT NULL;

-- Comentarios
COMMENT ON TABLE gym.Payments IS 'Registro de todos los pagos realizados en el sistema';
COMMENT ON COLUMN gym.Payments.member_id IS 'NULL para ventas anónimas (sin membresía)';

-- ============================================================================
-- SECCIÓN 8: TABLAS DE INVENTARIO Y PRODUCTOS
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: ProductCategories
-- Descripción: Categorías de productos
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.ProductCategories (
    category_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    category_name VARCHAR(50) NOT NULL,
    category_code VARCHAR(20) NOT NULL,
    description VARCHAR(255) NULL,
    parent_category_id UUID NULL,
    icon_name VARCHAR(50) NULL,
    color_code VARCHAR(7) DEFAULT '#6B7280',
    sort_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    -- Restricciones
    CONSTRAINT uq_productcategories_category_name UNIQUE (category_name),
    CONSTRAINT uq_productcategories_category_code UNIQUE (category_code),
    CONSTRAINT fk_productcategories_parent FOREIGN KEY (parent_category_id) 
        REFERENCES gym.ProductCategories(category_id) ON DELETE SET NULL
);

-- Comentarios
COMMENT ON TABLE gym.ProductCategories IS 'Categorías de productos (Bebidas, Suplementos, Ropa, Rentas, etc.)';
COMMENT ON COLUMN gym.ProductCategories.parent_category_id IS 'Permite subcategorías anidadas';

-- -----------------------------------------------------------------------------
-- Tabla: Products
-- Descripción: Catálogo de productos para venta
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Products (
    product_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    category_id UUID NOT NULL,
    product_code VARCHAR(50) NOT NULL,
    barcode VARCHAR(50) NULL,
    product_name VARCHAR(100) NOT NULL,
    description TEXT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    cost_price DECIMAL(10,2) NULL,
    tax_rate DECIMAL(5,2) DEFAULT 16.00,
    is_taxable BOOLEAN DEFAULT TRUE,
    is_rentable BOOLEAN DEFAULT FALSE,
    rental_price_per_day DECIMAL(10,2) NULL,
    rental_deposit DECIMAL(10,2) NULL,
    requires_return BOOLEAN DEFAULT FALSE,
    max_rental_days INTEGER NULL,
    unit_of_measure VARCHAR(20) DEFAULT 'UNIT',
    min_stock_alert INTEGER DEFAULT 5,
    image_path VARCHAR(500) NULL,
    sort_order INTEGER DEFAULT 0,
    is_featured BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NULL,
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    deleted_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT uq_products_product_code UNIQUE (product_code),
    CONSTRAINT uq_products_barcode UNIQUE (barcode),
    CONSTRAINT fk_products_category FOREIGN KEY (category_id) 
        REFERENCES gym.ProductCategories(category_id) ON DELETE RESTRICT,
    CONSTRAINT fk_products_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_products_unit_price CHECK (unit_price >= 0),
    CONSTRAINT chk_products_cost_price CHECK (cost_price IS NULL OR cost_price >= 0),
    CONSTRAINT chk_products_tax_rate CHECK (tax_rate >= 0 AND tax_rate <= 100)
);

-- Índices para Products
CREATE INDEX IF NOT EXISTS idx_products_category_id ON gym.Products(category_id);
CREATE INDEX IF NOT EXISTS idx_products_product_code ON gym.Products(product_code);
CREATE INDEX IF NOT EXISTS idx_products_barcode ON gym.Products(barcode) WHERE barcode IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_products_is_active ON gym.Products(is_active) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_products_is_rentable ON gym.Products(is_rentable) WHERE is_rentable = TRUE;

-- Comentarios
COMMENT ON TABLE gym.Products IS 'Catálogo de productos para venta y renta';
COMMENT ON COLUMN gym.Products.is_rentable IS 'Indica si el producto es para renta (toallas, lockers, etc.)';
COMMENT ON COLUMN gym.Products.requires_return IS 'Indica si el producto rentado debe ser devuelto';

-- -----------------------------------------------------------------------------
-- Tabla: InventoryStock
-- Descripción: Stock de productos por sucursal
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.InventoryStock (
    stock_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL,
    branch_id UUID NOT NULL,
    quantity_available INTEGER NOT NULL DEFAULT 0,
    quantity_reserved INTEGER DEFAULT 0,
    quantity_damaged INTEGER DEFAULT 0,
    min_stock_level INTEGER DEFAULT 5,
    max_stock_level INTEGER NULL,
    reorder_point INTEGER NULL,
    reorder_quantity INTEGER NULL,
    last_restock_at TIMESTAMP WITH TIME ZONE NULL,
    last_restock_quantity INTEGER NULL,
    last_count_at TIMESTAMP WITH TIME ZONE NULL,
    last_count_by UUID NULL,
    location_in_store VARCHAR(50) NULL,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    -- Restricciones
    CONSTRAINT uq_inventorystock_product_branch UNIQUE (product_id, branch_id),
    CONSTRAINT fk_inventorystock_product FOREIGN KEY (product_id) 
        REFERENCES gym.Products(product_id) ON DELETE CASCADE,
    CONSTRAINT fk_inventorystock_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE CASCADE,
    CONSTRAINT fk_inventorystock_last_count_by FOREIGN KEY (last_count_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_inventorystock_quantities CHECK (
        quantity_available >= 0 AND 
        quantity_reserved >= 0 AND 
        quantity_damaged >= 0
    )
);

-- Índices para InventoryStock
CREATE INDEX IF NOT EXISTS idx_inventorystock_product_id ON gym.InventoryStock(product_id);
CREATE INDEX IF NOT EXISTS idx_inventorystock_branch_id ON gym.InventoryStock(branch_id);
CREATE INDEX IF NOT EXISTS idx_inventorystock_low_stock ON gym.InventoryStock(quantity_available) 
    WHERE quantity_available <= min_stock_level;

-- Comentarios
COMMENT ON TABLE gym.InventoryStock IS 'Control de inventario por producto y sucursal';
COMMENT ON COLUMN gym.InventoryStock.quantity_reserved IS 'Cantidad reservada (en rentas activas)';

-- ============================================================================
-- SECCIÓN 9: TABLAS DE VENTAS
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: Sales
-- Descripción: Encabezado de ventas/tickets
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.Sales (
    sale_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NOT NULL,
    member_id UUID NULL,
    ticket_number VARCHAR(30) NOT NULL,
    sale_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    subtotal DECIMAL(10,2) NOT NULL,
    discount_amount DECIMAL(10,2) DEFAULT 0,
    discount_percent DECIMAL(5,2) DEFAULT 0,
    discount_reason VARCHAR(255) NULL,
    tax_amount DECIMAL(10,2) DEFAULT 0,
    total_amount DECIMAL(10,2) NOT NULL,
    amount_paid DECIMAL(10,2) DEFAULT 0,
    change_amount DECIMAL(10,2) DEFAULT 0,
    status sale_status_enum DEFAULT 'COMPLETED',
    cancelled_at TIMESTAMP WITH TIME ZONE NULL,
    cancelled_by UUID NULL,
    cancellation_reason VARCHAR(255) NULL,
    invoice_number VARCHAR(50) NULL,
    invoice_requested BOOLEAN DEFAULT FALSE,
    notes TEXT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    
    -- Restricciones
    CONSTRAINT uq_sales_ticket_number UNIQUE (ticket_number),
    CONSTRAINT fk_sales_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE RESTRICT,
    CONSTRAINT fk_sales_member FOREIGN KEY (member_id) 
        REFERENCES gym.Members(member_id) ON DELETE SET NULL,
    CONSTRAINT fk_sales_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE RESTRICT,
    CONSTRAINT fk_sales_cancelled_by FOREIGN KEY (cancelled_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_sales_amounts CHECK (
        subtotal >= 0 AND 
        discount_amount >= 0 AND 
        tax_amount >= 0 AND 
        total_amount >= 0
    )
);

-- Índices para Sales
CREATE INDEX IF NOT EXISTS idx_sales_branch_id ON gym.Sales(branch_id);
CREATE INDEX IF NOT EXISTS idx_sales_member_id ON gym.Sales(member_id);
CREATE INDEX IF NOT EXISTS idx_sales_ticket_number ON gym.Sales(ticket_number);
CREATE INDEX IF NOT EXISTS idx_sales_sale_date ON gym.Sales(sale_date);
CREATE INDEX IF NOT EXISTS idx_sales_status ON gym.Sales(status);
CREATE INDEX IF NOT EXISTS idx_sales_created_by ON gym.Sales(created_by);

-- Agregar FK de sale_id a Payments
ALTER TABLE gym.Payments 
    ADD CONSTRAINT fk_payments_sale FOREIGN KEY (sale_id) 
        REFERENCES gym.Sales(sale_id) ON DELETE SET NULL;

-- Comentarios
COMMENT ON TABLE gym.Sales IS 'Encabezado de ventas/tickets de venta';
COMMENT ON COLUMN gym.Sales.member_id IS 'NULL para ventas a clientes no registrados';
COMMENT ON COLUMN gym.Sales.ticket_number IS 'Número de ticket único para impresión';

-- -----------------------------------------------------------------------------
-- Tabla: SaleItems
-- Descripción: Detalle de productos en cada venta
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.SaleItems (
    sale_item_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sale_id UUID NOT NULL,
    product_id UUID NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    unit_price DECIMAL(10,2) NOT NULL,
    cost_price_at_sale DECIMAL(10,2) NULL,
    discount_percent DECIMAL(5,2) DEFAULT 0,
    discount_amount DECIMAL(10,2) DEFAULT 0,
    tax_amount DECIMAL(10,2) DEFAULT 0,
    line_total DECIMAL(10,2) NOT NULL,
    is_rental BOOLEAN DEFAULT FALSE,
    rental_start_date DATE NULL,
    rental_expected_return DATE NULL,
    rental_actual_return TIMESTAMP WITH TIME ZONE NULL,
    rental_returned_to UUID NULL,
    rental_deposit_paid DECIMAL(10,2) NULL,
    rental_deposit_returned DECIMAL(10,2) NULL,
    rental_condition_notes VARCHAR(255) NULL,
    notes VARCHAR(255) NULL,
    
    -- Restricciones
    CONSTRAINT fk_saleitems_sale FOREIGN KEY (sale_id) 
        REFERENCES gym.Sales(sale_id) ON DELETE CASCADE,
    CONSTRAINT fk_saleitems_product FOREIGN KEY (product_id) 
        REFERENCES gym.Products(product_id) ON DELETE RESTRICT,
    CONSTRAINT fk_saleitems_rental_returned_to FOREIGN KEY (rental_returned_to) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_saleitems_quantity CHECK (quantity > 0),
    CONSTRAINT chk_saleitems_prices CHECK (unit_price >= 0 AND line_total >= 0)
);

-- Índices para SaleItems
CREATE INDEX IF NOT EXISTS idx_saleitems_sale_id ON gym.SaleItems(sale_id);
CREATE INDEX IF NOT EXISTS idx_saleitems_product_id ON gym.SaleItems(product_id);
CREATE INDEX IF NOT EXISTS idx_saleitems_is_rental ON gym.SaleItems(is_rental) WHERE is_rental = TRUE;
CREATE INDEX IF NOT EXISTS idx_saleitems_pending_rentals ON gym.SaleItems(rental_expected_return) 
    WHERE is_rental = TRUE AND rental_actual_return IS NULL;

-- Comentarios
COMMENT ON TABLE gym.SaleItems IS 'Detalle de productos vendidos o rentados en cada venta';
COMMENT ON COLUMN gym.SaleItems.is_rental IS 'Indica si es una renta en lugar de venta';
COMMENT ON COLUMN gym.SaleItems.cost_price_at_sale IS 'Costo del producto al momento de la venta (para reportes de utilidad)';

-- ============================================================================
-- SECCIÓN 10: TABLAS DE CLASES Y HORARIOS
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Tabla: ClassTypes
-- Descripción: Tipos/Categorías de clases grupales
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.ClassTypes (
    class_type_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    type_name VARCHAR(50) NOT NULL,
    type_code VARCHAR(20) NOT NULL,
    description TEXT NULL,
    default_duration_minutes INTEGER DEFAULT 60,
    default_capacity INTEGER DEFAULT 20,
    difficulty_level VARCHAR(20) DEFAULT 'INTERMEDIATE',
    calories_burned_estimate INTEGER NULL,
    equipment_needed TEXT NULL,
    color_code VARCHAR(7) DEFAULT '#3B82F6',
    icon_name VARCHAR(50) NULL,
    image_path VARCHAR(500) NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    -- Restricciones
    CONSTRAINT uq_classtypes_type_name UNIQUE (type_name),
    CONSTRAINT uq_classtypes_type_code UNIQUE (type_code),
    CONSTRAINT chk_classtypes_duration CHECK (default_duration_minutes > 0),
    CONSTRAINT chk_classtypes_capacity CHECK (default_capacity > 0),
    CONSTRAINT chk_classtypes_color CHECK (color_code ~ '^#[0-9A-Fa-f]{6}$')
);

-- Comentarios
COMMENT ON TABLE gym.ClassTypes IS 'Catálogo de tipos de clases (Spinning, Yoga, CrossFit, etc.)';
COMMENT ON COLUMN gym.ClassTypes.difficulty_level IS 'Nivel: BEGINNER, INTERMEDIATE, ADVANCED, ALL_LEVELS';

-- -----------------------------------------------------------------------------
-- Tabla: ClassSchedules
-- Descripción: Programación de clases
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.ClassSchedules (
    schedule_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    branch_id UUID NOT NULL,
    class_type_id UUID NOT NULL,
    trainer_id UUID NOT NULL,
    class_name VARCHAR(100) NULL,
    day_of_week SMALLINT NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    capacity INTEGER NOT NULL,
    waitlist_capacity INTEGER DEFAULT 5,
    room_location VARCHAR(50) NULL,
    is_recurring BOOLEAN DEFAULT TRUE,
    specific_date DATE NULL,
    is_active BOOLEAN DEFAULT TRUE,
    valid_from DATE NULL,
    valid_until DATE NULL,
    cancellation_notice_hours INTEGER DEFAULT 2,
    allow_online_booking BOOLEAN DEFAULT TRUE,
    notes TEXT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    created_by UUID NULL,
    updated_at TIMESTAMP WITH TIME ZONE NULL,
    
    -- Restricciones
    CONSTRAINT fk_classschedules_branch FOREIGN KEY (branch_id) 
        REFERENCES gym.Branches(branch_id) ON DELETE CASCADE,
    CONSTRAINT fk_classschedules_class_type FOREIGN KEY (class_type_id) 
        REFERENCES gym.ClassTypes(class_type_id) ON DELETE RESTRICT,
    CONSTRAINT fk_classschedules_trainer FOREIGN KEY (trainer_id) 
        REFERENCES gym.Users(user_id) ON DELETE RESTRICT,
    CONSTRAINT fk_classschedules_created_by FOREIGN KEY (created_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_classschedules_day_of_week CHECK (day_of_week BETWEEN 0 AND 6),
    CONSTRAINT chk_classschedules_times CHECK (end_time > start_time),
    CONSTRAINT chk_classschedules_capacity CHECK (capacity > 0)
);

-- Índices para ClassSchedules
CREATE INDEX IF NOT EXISTS idx_classschedules_branch_id ON gym.ClassSchedules(branch_id);
CREATE INDEX IF NOT EXISTS idx_classschedules_class_type_id ON gym.ClassSchedules(class_type_id);
CREATE INDEX IF NOT EXISTS idx_classschedules_trainer_id ON gym.ClassSchedules(trainer_id);
CREATE INDEX IF NOT EXISTS idx_classschedules_day_time ON gym.ClassSchedules(day_of_week, start_time);
CREATE INDEX IF NOT EXISTS idx_classschedules_is_active ON gym.ClassSchedules(is_active) WHERE is_active = TRUE;

-- Comentarios
COMMENT ON TABLE gym.ClassSchedules IS 'Programación de clases por día de la semana';
COMMENT ON COLUMN gym.ClassSchedules.day_of_week IS '0=Domingo, 1=Lunes, ..., 6=Sábado';
COMMENT ON COLUMN gym.ClassSchedules.specific_date IS 'Para clases únicas no recurrentes';

-- -----------------------------------------------------------------------------
-- Tabla: ClassEnrollments
-- Descripción: Inscripciones de miembros a clases
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS gym.ClassEnrollments (
    enrollment_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    schedule_id UUID NOT NULL,
    member_id UUID NOT NULL,
    class_date DATE NOT NULL,
    enrolled_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    enrolled_by UUID NULL,
    status enrollment_status_enum DEFAULT 'ENROLLED',
    waitlist_position INTEGER NULL,
    attended_at TIMESTAMP WITH TIME ZONE NULL,
    cancelled_at TIMESTAMP WITH TIME ZONE NULL,
    cancelled_by UUID NULL,
    cancellation_reason VARCHAR(255) NULL,
    no_show_notified BOOLEAN DEFAULT FALSE,
    rating SMALLINT NULL,
    feedback TEXT NULL,
    notes VARCHAR(255) NULL,
    
    -- Restricciones
    CONSTRAINT uq_classenrollments_schedule_member_date UNIQUE (schedule_id, member_id, class_date),
    CONSTRAINT fk_classenrollments_schedule FOREIGN KEY (schedule_id) 
        REFERENCES gym.ClassSchedules(schedule_id) ON DELETE CASCADE,
    CONSTRAINT fk_classenrollments_member FOREIGN KEY (member_id) 
        REFERENCES gym.Members(member_id) ON DELETE CASCADE,
    CONSTRAINT fk_classenrollments_enrolled_by FOREIGN KEY (enrolled_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_classenrollments_cancelled_by FOREIGN KEY (cancelled_by) 
        REFERENCES gym.Users(user_id) ON DELETE SET NULL,
    CONSTRAINT chk_classenrollments_rating CHECK (rating IS NULL OR (rating >= 1 AND rating <= 5))
);

-- Índices para ClassEnrollments
CREATE INDEX IF NOT EXISTS idx_classenrollments_schedule_id ON gym.ClassEnrollments(schedule_id);
CREATE INDEX IF NOT EXISTS idx_classenrollments_member_id ON gym.ClassEnrollments(member_id);
CREATE INDEX IF NOT EXISTS idx_classenrollments_class_date ON gym.ClassEnrollments(class_date);
CREATE INDEX IF NOT EXISTS idx_classenrollments_status ON gym.ClassEnrollments(status);
CREATE INDEX IF NOT EXISTS idx_classenrollments_schedule_date ON gym.ClassEnrollments(schedule_id, class_date);

-- Comentarios
COMMENT ON TABLE gym.ClassEnrollments IS 'Inscripciones de miembros a clases específicas';
COMMENT ON COLUMN gym.ClassEnrollments.waitlist_position IS 'Posición en lista de espera (NULL si está inscrito)';
COMMENT ON COLUMN gym.ClassEnrollments.rating IS 'Calificación del 1 al 5 dada por el miembro';

-- ============================================================================
-- SECCIÓN 11: DATOS INICIALES (SEED DATA)
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Roles del Sistema
-- -----------------------------------------------------------------------------
INSERT INTO gym.Roles (role_name, role_code, description, permission_level, is_system_role, is_active)
VALUES 
    ('Administrator', 'ADMIN', 'Acceso completo al sistema - gestión total', 100, TRUE, TRUE),
    ('Staff', 'STAFF', 'Personal de recepción y ventas', 50, TRUE, TRUE),
    ('Trainer', 'TRAINER', 'Entrenador - gestión de clases y miembros asignados', 30, TRUE, TRUE),
    ('Client', 'CLIENT', 'Cliente/Miembro con acceso limitado (futuro portal)', 10, TRUE, TRUE)
ON CONFLICT (role_code) DO NOTHING;

-- -----------------------------------------------------------------------------
-- Métodos de Pago
-- -----------------------------------------------------------------------------
INSERT INTO gym.PaymentMethods (method_name, method_code, description, requires_reference, sort_order, is_active)
VALUES 
    ('Efectivo', 'CASH', 'Pago en efectivo', FALSE, 1, TRUE),
    ('Tarjeta de Crédito', 'CARD_CREDIT', 'Pago con tarjeta de crédito', TRUE, 2, TRUE),
    ('Tarjeta de Débito', 'CARD_DEBIT', 'Pago con tarjeta de débito', TRUE, 3, TRUE),
    ('Transferencia Bancaria', 'TRANSFER', 'Transferencia electrónica', TRUE, 4, TRUE),
    ('Depósito Bancario', 'DEPOSIT', 'Depósito en sucursal bancaria', TRUE, 5, TRUE),
    ('Monedero Electrónico', 'DIGITAL', 'PayPal, MercadoPago, etc.', TRUE, 6, TRUE)
ON CONFLICT (method_code) DO NOTHING;

-- -----------------------------------------------------------------------------
-- Categorías de Productos
-- -----------------------------------------------------------------------------
INSERT INTO gym.ProductCategories (category_name, category_code, description, color_code, sort_order, is_active)
VALUES 
    ('Bebidas', 'BEVERAGES', 'Agua, bebidas deportivas, jugos', '#3B82F6', 1, TRUE),
    ('Suplementos', 'SUPPLEMENTS', 'Proteínas, vitaminas, aminoácidos', '#10B981', 2, TRUE),
    ('Ropa Deportiva', 'APPAREL', 'Playeras, shorts, pants, accesorios', '#8B5CF6', 3, TRUE),
    ('Accesorios', 'ACCESSORIES', 'Guantes, cinturones, muñequeras', '#F59E0B', 4, TRUE),
    ('Artículos de Renta', 'RENTALS', 'Toallas, candados, lockers', '#EF4444', 5, TRUE),
    ('Servicios Adicionales', 'SERVICES', 'Entrenamiento personal, nutrición', '#EC4899', 6, TRUE)
ON CONFLICT (category_code) DO NOTHING;

-- -----------------------------------------------------------------------------
-- Tipos de Clases
-- -----------------------------------------------------------------------------
INSERT INTO gym.ClassTypes (type_name, type_code, description, default_duration_minutes, default_capacity, difficulty_level, color_code, is_active)
VALUES 
    ('Spinning', 'SPINNING', 'Ciclismo indoor de alta intensidad', 45, 25, 'INTERMEDIATE', '#EF4444', TRUE),
    ('Yoga', 'YOGA', 'Práctica de posturas y meditación', 60, 20, 'ALL_LEVELS', '#10B981', TRUE),
    ('Zumba', 'ZUMBA', 'Baile fitness con ritmos latinos', 60, 30, 'BEGINNER', '#F59E0B', TRUE),
    ('CrossFit', 'CROSSFIT', 'Entrenamiento funcional de alta intensidad', 60, 15, 'ADVANCED', '#8B5CF6', TRUE),
    ('Pilates', 'PILATES', 'Fortalecimiento del core y flexibilidad', 50, 15, 'INTERMEDIATE', '#3B82F6', TRUE),
    ('Box Fitness', 'BOXING', 'Cardio boxing sin contacto', 45, 20, 'INTERMEDIATE', '#1F2937', TRUE),
    ('Funcional', 'FUNCTIONAL', 'Entrenamiento con peso corporal y accesorios', 45, 20, 'ALL_LEVELS', '#059669', TRUE),
    ('Stretching', 'STRETCHING', 'Estiramientos y relajación', 30, 25, 'BEGINNER', '#6366F1', TRUE)
ON CONFLICT (type_code) DO NOTHING;

-- ============================================================================
-- SECCIÓN 12: FUNCIONES AUXILIARES
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Función: Generar código de miembro
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.generate_member_code(p_branch_code VARCHAR)
RETURNS VARCHAR AS $$
DECLARE
    v_sequence INTEGER;
    v_code VARCHAR(20);
BEGIN
    -- Obtener el siguiente número de secuencia para la sucursal
    SELECT COALESCE(MAX(CAST(SUBSTRING(member_code FROM '[0-9]+$') AS INTEGER)), 0) + 1
    INTO v_sequence
    FROM gym.Members
    WHERE member_code LIKE p_branch_code || '-%';
    
    -- Formatear el código
    v_code := p_branch_code || '-' || LPAD(v_sequence::TEXT, 5, '0');
    
    RETURN v_code;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION gym.generate_member_code IS 'Genera código único de miembro basado en la sucursal';

-- -----------------------------------------------------------------------------
-- Función: Generar número de ticket
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.generate_ticket_number(p_branch_code VARCHAR)
RETURNS VARCHAR AS $$
DECLARE
    v_date_part VARCHAR(8);
    v_sequence INTEGER;
    v_ticket VARCHAR(30);
BEGIN
    -- Formato: BRANCH-YYYYMMDD-NNNN
    v_date_part := TO_CHAR(CURRENT_DATE, 'YYYYMMDD');
    
    -- Obtener secuencia del día
    SELECT COALESCE(MAX(CAST(SUBSTRING(ticket_number FROM '[0-9]+$') AS INTEGER)), 0) + 1
    INTO v_sequence
    FROM gym.Sales
    WHERE ticket_number LIKE p_branch_code || '-' || v_date_part || '-%'
    AND DATE(sale_date) = CURRENT_DATE;
    
    v_ticket := p_branch_code || '-' || v_date_part || '-' || LPAD(v_sequence::TEXT, 4, '0');
    
    RETURN v_ticket;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION gym.generate_ticket_number IS 'Genera número de ticket único por sucursal y día';

-- -----------------------------------------------------------------------------
-- Función: Calcular edad
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.calculate_age(p_birth_date DATE)
RETURNS INTEGER AS $$
BEGIN
    IF p_birth_date IS NULL THEN
        RETURN NULL;
    END IF;
    RETURN EXTRACT(YEAR FROM AGE(CURRENT_DATE, p_birth_date))::INTEGER;
END;
$$ LANGUAGE plpgsql IMMUTABLE;

COMMENT ON FUNCTION gym.calculate_age IS 'Calcula la edad en años a partir de fecha de nacimiento';

-- -----------------------------------------------------------------------------
-- Función: Verificar membresía activa
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.has_active_membership(p_member_id UUID)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 
        FROM gym.Memberships 
        WHERE member_id = p_member_id 
        AND status = 'ACTIVE' 
        AND CURRENT_DATE BETWEEN start_date AND end_date
    );
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION gym.has_active_membership IS 'Verifica si un miembro tiene membresía activa vigente';

-- ============================================================================
-- SECCIÓN 13: TRIGGERS
-- ============================================================================

-- -----------------------------------------------------------------------------
-- Trigger: Actualizar updated_at automáticamente
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Aplicar trigger a tablas principales
DO $$
DECLARE
    t TEXT;
BEGIN
    FOREACH t IN ARRAY ARRAY[
        'Licenses', 'Branches', 'BranchSettings', 'Users', 'Members', 
        'MembershipPlans', 'Memberships', 'Products', 'InventoryStock', 'ClassSchedules'
    ]
    LOOP
        EXECUTE format('
            DROP TRIGGER IF EXISTS trg_%s_updated_at ON gym.%s;
            CREATE TRIGGER trg_%s_updated_at
                BEFORE UPDATE ON gym.%s
                FOR EACH ROW
                EXECUTE FUNCTION gym.update_updated_at_column();
        ', lower(t), t, lower(t), t);
    END LOOP;
END $$;

-- -----------------------------------------------------------------------------
-- Trigger: Crear BranchSettings automáticamente al crear Branch
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.create_branch_settings()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO gym.BranchSettings (branch_id, business_name)
    VALUES (NEW.branch_id, NEW.branch_name);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_branches_create_settings ON gym.Branches;
CREATE TRIGGER trg_branches_create_settings
    AFTER INSERT ON gym.Branches
    FOR EACH ROW
    EXECUTE FUNCTION gym.create_branch_settings();

-- -----------------------------------------------------------------------------
-- Trigger: Actualizar stock al registrar venta
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.update_stock_on_sale()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        -- Reducir stock disponible
        UPDATE gym.InventoryStock
        SET quantity_available = quantity_available - NEW.quantity,
            quantity_reserved = CASE WHEN NEW.is_rental THEN quantity_reserved + NEW.quantity ELSE quantity_reserved END,
            updated_at = NOW()
        WHERE product_id = NEW.product_id
        AND branch_id = (SELECT branch_id FROM gym.Sales WHERE sale_id = NEW.sale_id);
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_saleitems_update_stock ON gym.SaleItems;
CREATE TRIGGER trg_saleitems_update_stock
    AFTER INSERT ON gym.SaleItems
    FOR EACH ROW
    EXECUTE FUNCTION gym.update_stock_on_sale();

-- -----------------------------------------------------------------------------
-- Trigger: Calcular duración de asistencia al hacer check-out
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION gym.calculate_attendance_duration()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.check_out_at IS NOT NULL AND OLD.check_out_at IS NULL THEN
        NEW.duration_minutes = EXTRACT(EPOCH FROM (NEW.check_out_at - NEW.check_in_at)) / 60;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_attendances_duration ON gym.Attendances;
CREATE TRIGGER trg_attendances_duration
    BEFORE UPDATE ON gym.Attendances
    FOR EACH ROW
    EXECUTE FUNCTION gym.calculate_attendance_duration();

-- ============================================================================
-- SECCIÓN 14: PERMISOS
-- ============================================================================

-- Otorgar permisos al usuario de la aplicación
GRANT USAGE ON SCHEMA gym TO gym_app_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA gym TO gym_app_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA gym TO gym_app_user;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA gym TO gym_app_user;

-- Permisos para futuras tablas
ALTER DEFAULT PRIVILEGES IN SCHEMA gym GRANT ALL ON TABLES TO gym_app_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA gym GRANT ALL ON SEQUENCES TO gym_app_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA gym GRANT EXECUTE ON FUNCTIONS TO gym_app_user;

-- ============================================================================
-- SECCIÓN 15: VERIFICACIÓN FINAL
-- ============================================================================

-- Mostrar resumen de objetos creados
DO $$
DECLARE
    v_tables INTEGER;
    v_indexes INTEGER;
    v_functions INTEGER;
    v_triggers INTEGER;
BEGIN
    SELECT COUNT(*) INTO v_tables FROM information_schema.tables WHERE table_schema = 'gym';
    SELECT COUNT(*) INTO v_indexes FROM pg_indexes WHERE schemaname = 'gym';
    SELECT COUNT(*) INTO v_functions FROM information_schema.routines WHERE routine_schema = 'gym';
    SELECT COUNT(*) INTO v_triggers FROM information_schema.triggers WHERE trigger_schema = 'gym';
    
    RAISE NOTICE '';
    RAISE NOTICE '============================================';
    RAISE NOTICE '  GYM MANAGER - SCHEMA CREADO EXITOSAMENTE';
    RAISE NOTICE '============================================';
    RAISE NOTICE '  Tablas creadas:     %', v_tables;
    RAISE NOTICE '  Índices creados:    %', v_indexes;
    RAISE NOTICE '  Funciones creadas:  %', v_functions;
    RAISE NOTICE '  Triggers creados:   %', v_triggers;
    RAISE NOTICE '============================================';
    RAISE NOTICE '';
END $$;

-- Listar todas las tablas creadas
SELECT 
    table_name AS "Tabla",
    (SELECT COUNT(*) FROM information_schema.columns c WHERE c.table_schema = t.table_schema AND c.table_name = t.table_name) AS "Columnas"
FROM information_schema.tables t
WHERE table_schema = 'gym'
ORDER BY table_name;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
