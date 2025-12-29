-- Drop tables if they exist (order matters because of FKs)
IF OBJECT_ID('dbo.Bids', 'U') IS NOT NULL DROP TABLE dbo.Bids;
IF OBJECT_ID('dbo.WatchImages', 'U') IS NOT NULL DROP TABLE dbo.WatchImages;
IF OBJECT_ID('dbo.Advertisements', 'U') IS NOT NULL DROP TABLE dbo.Advertisements;
IF OBJECT_ID('dbo.Watches', 'U') IS NOT NULL DROP TABLE dbo.Watches;
GO

-- Watches
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

-- Advertisements
CREATE TABLE dbo.Advertisements
(
    AdvertisementId   UNIQUEIDENTIFIER   NOT NULL,
    WatchId           UNIQUEIDENTIFIER   NOT NULL,
    SellerUserId      UNIQUEIDENTIFIER   NOT NULL,
    Title             NVARCHAR(200)      NOT NULL,
    Description       NVARCHAR(MAX)      NULL,
    AskingPrice       DECIMAL(18, 2)     NOT NULL,
    Status            NVARCHAR(50)       NOT NULL,           -- non-null
    ViewCount         INT                NOT NULL,           -- treat as required
    PublishedAt       DATETIMEOFFSET(7)  NULL,               -- nullable if watch still a draft
    ExpiresAt         DATETIMEOFFSET(7)  NULL,           -- nullable only filled in when also published
    SoldAt            DATETIMEOFFSET(7)  NULL,
    CreatedAt         DATETIMEOFFSET(7)  NOT NULL,           -- non-null
    UpdatedAt         DATETIMEOFFSET(7)  NOT NULL,           -- non-null
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

-- WatchImages
CREATE TABLE dbo.WatchImages
(
    ImageId      UNIQUEIDENTIFIER   NOT NULL,
    WatchId      UNIQUEIDENTIFIER   NOT NULL,
    BlobUrl      NVARCHAR(500)      NOT NULL,
    FileName     NVARCHAR(255)      NOT NULL,
    FileSize     BIGINT             NULL,
    ContentType  NVARCHAR(100)      NULL,
    IsPrimary    BIT                NOT NULL,           -- treat as required
    UploadedAt   DATETIMEOFFSET(7)  NOT NULL,           -- non-null

    CONSTRAINT PK_WatchImages PRIMARY KEY (ImageId),

    CONSTRAINT FK_WatchImages_Watches
        FOREIGN KEY (WatchId) REFERENCES dbo.Watches (WatchId)
);
GO

-- Indexes on WatchImages
CREATE INDEX IX_WatchImages_WatchId
    ON dbo.WatchImages (WatchId);
GO

-- Bids
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
