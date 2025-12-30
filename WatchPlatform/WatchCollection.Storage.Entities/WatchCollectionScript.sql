-- =========================================
-- Drop tables if they exist (order matters)
-- =========================================
IF OBJECT_ID('dbo.Bids', 'U') IS NOT NULL DROP TABLE dbo.Bids;
IF OBJECT_ID('dbo.WatchImages', 'U') IS NOT NULL DROP TABLE dbo.WatchImages;
IF OBJECT_ID('dbo.Advertisements', 'U') IS NOT NULL DROP TABLE dbo.Advertisements;
IF OBJECT_ID('dbo.Watches', 'U') IS NOT NULL DROP TABLE dbo.Watches;
GO

-- =========
-- Watches
-- =========
CREATE TABLE dbo.Watches
(
    WatchId           UNIQUEIDENTIFIER   NOT NULL,
    OwnerUserId       UNIQUEIDENTIFIER   NOT NULL,
    Brand             NVARCHAR(100)      NOT NULL,
    Model             NVARCHAR(200)      NOT NULL,
    ReferenceNumber   NVARCHAR(100)      NULL,
    SerialNumber      NVARCHAR(100)      NULL,
    YearOfProduction  INT                NULL,
    Condition         NVARCHAR(50)       NOT NULL,
    Description       NVARCHAR(MAX)      NULL,
    PurchasePrice     DECIMAL(18, 2)     NULL,
    PurchaseDate      DATE               NULL,  -- DateOnly
    CreatedAt         DATETIMEOFFSET(7)  NOT NULL,
    UpdatedAt         DATETIMEOFFSET(7)  NOT NULL,

    CONSTRAINT PK_Watches PRIMARY KEY (WatchId)
);
GO

-- Indexes on Watches
CREATE INDEX IX_Watches_Brand
    ON dbo.Watches (Brand);
GO

CREATE INDEX IX_Watches_OwnerUserId
    ON dbo.Watches (OwnerUserId);
GO

-- Defaults for Watches timestamps
ALTER TABLE dbo.Watches
ADD CONSTRAINT DF_Watches_CreatedAt
    DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;
GO

ALTER TABLE dbo.Watches
ADD CONSTRAINT DF_Watches_UpdatedAt
    DEFAULT (SYSUTCDATETIME()) FOR UpdatedAt;
GO

-- =========
-- Advertisements
-- =========
CREATE TABLE dbo.Advertisements
(
    AdvertisementId   UNIQUEIDENTIFIER   NOT NULL,
    WatchId           UNIQUEIDENTIFIER   NOT NULL,
    SellerUserId      UNIQUEIDENTIFIER   NOT NULL,
    Title             NVARCHAR(200)      NOT NULL,
    Description       NVARCHAR(MAX)      NULL,
    AskingPrice       DECIMAL(18, 2)     NOT NULL,
    Status            NVARCHAR(50)       NOT NULL,
    ViewCount         INT                NOT NULL,
    PublishedAt       DATETIMEOFFSET(7)  NULL,
    ExpiresAt         DATETIMEOFFSET(7)  NULL,
    SoldAt            DATETIMEOFFSET(7)  NULL,
    CreatedAt         DATETIMEOFFSET(7)  NOT NULL,
    UpdatedAt         DATETIMEOFFSET(7)  NOT NULL,
    AllowBids         BIT                NOT NULL,

    CONSTRAINT PK_Advertisements PRIMARY KEY (AdvertisementId),

    CONSTRAINT FK_Advertisements_Watches
        FOREIGN KEY (WatchId) REFERENCES dbo.Watches (WatchId)
);
GO

-- Indexes on Advertisements
CREATE INDEX IX_Advertisements_SellerUserId
    ON dbo.Advertisements (SellerUserId);
GO

CREATE INDEX IX_Advertisements_Status
    ON dbo.Advertisements (Status);
GO

CREATE INDEX IX_Advertisements_WatchId
    ON dbo.Advertisements (WatchId);
GO

-- Defaults for Advertisements timestamps
ALTER TABLE dbo.Advertisements
ADD CONSTRAINT DF_Advertisements_CreatedAt
    DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;
GO

ALTER TABLE dbo.Advertisements
ADD CONSTRAINT DF_Advertisements_UpdatedAt
    DEFAULT (SYSUTCDATETIME()) FOR UpdatedAt;
GO

-- =========
-- WatchImages
-- =========
CREATE TABLE dbo.WatchImages
(
    ImageId      UNIQUEIDENTIFIER   NOT NULL,
    WatchId      UNIQUEIDENTIFIER   NOT NULL,
    BlobUrl      NVARCHAR(500)      NOT NULL,
    FileName     NVARCHAR(255)      NOT NULL,
    FileSize     BIGINT             NULL,
    ContentType  NVARCHAR(100)      NULL,
    IsPrimary    BIT                NOT NULL,
    UploadedAt   DATETIMEOFFSET(7)  NOT NULL,

    CONSTRAINT PK_WatchImages PRIMARY KEY (ImageId),

    CONSTRAINT FK_WatchImages_Watches
        FOREIGN KEY (WatchId) REFERENCES dbo.Watches (WatchId)
);
GO

-- Indexes on WatchImages
CREATE INDEX IX_WatchImages_WatchId
    ON dbo.WatchImages (WatchId);
GO

-- Defaults for WatchImages timestamps
ALTER TABLE dbo.WatchImages
ADD CONSTRAINT DF_WatchImages_UploadedAt
    DEFAULT (SYSUTCDATETIME()) FOR UploadedAt;
GO

-- =========
-- Bids
-- =========
CREATE TABLE dbo.Bids
(
    BidId           UNIQUEIDENTIFIER   NOT NULL,
    AdvertisementId UNIQUEIDENTIFIER   NOT NULL,
    BidderId        UNIQUEIDENTIFIER   NOT NULL,
    CreatedAt       DATETIMEOFFSET(7)  NOT NULL,
    Amount          DECIMAL(18, 2)     NOT NULL,

    CONSTRAINT PK_Bids PRIMARY KEY (BidId),

    CONSTRAINT FK_Bids_Advertisements
        FOREIGN KEY (AdvertisementId) REFERENCES dbo.Advertisements (AdvertisementId)
            ON DELETE NO ACTION
);
GO

-- Defaults for Bids timestamps
ALTER TABLE dbo.Bids
ADD CONSTRAINT DF_Bids_CreatedAt
    DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;
GO

-- =====================================================
-- Triggers to keep UpdatedAt / UploadedAt in sync
-- =====================================================

-- Watches: set UpdatedAt on UPDATE
CREATE TRIGGER TR_Watches_SetUpdatedAt
ON dbo.Watches
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE w
    SET UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Watches w
    INNER JOIN inserted i ON w.WatchId = i.WatchId;
END;
GO

-- Advertisements: set UpdatedAt on UPDATE
CREATE TRIGGER TR_Advertisements_SetUpdatedAt
ON dbo.Advertisements
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE a
    SET UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Advertisements a
    INNER JOIN inserted i ON a.AdvertisementId = i.AdvertisementId;
END;
GO

-- WatchImages: bump UploadedAt when record changes (optional)
CREATE TRIGGER TR_WatchImages_SetUploadedAt
ON dbo.WatchImages
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE wi
    SET UploadedAt = SYSUTCDATETIME()
    FROM dbo.WatchImages wi
    INNER JOIN inserted i ON wi.ImageId = i.ImageId;
END;
GO
