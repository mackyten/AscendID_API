using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AscendID_API.Domain.Entities;
using AscendID_API.Domain.Entities.Identity;

namespace AscendID_API.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TempTestRecord> TempTestRecords { get; set; }
    public DbSet<OAuthClient> OAuthClients { get; set; }
    public DbSet<OAuthScope> OAuthScopes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PostgreSQL schema
        modelBuilder.HasDefaultSchema("public");

        // Configure TempTestRecord entity
        ConfigureTempTestRecord(modelBuilder);
        
        // Configure Identity entities
        ConfigureIdentityEntities(modelBuilder);
        
        // Configure OAuth entities
        ConfigureOAuthEntities(modelBuilder);
    }

    private void ConfigureTempTestRecord(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TempTestRecord>(entity =>
        {
            entity.ToTable("temp_test_records"); // Use snake_case for PostgreSQL convention
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityByDefaultColumn(); // PostgreSQL identity column
            
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired()
                .HasDefaultValue(true);

            // Add indexes for better performance
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("ix_temp_test_records_is_active");
            
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("ix_temp_test_records_created_at");
        });
    }

    private void ConfigureIdentityEntities(ModelBuilder modelBuilder)
    {
        // Configure ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("users");
            
            entity.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(e => e.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();
            
            entity.Property(e => e.LastLoginAt)
                .HasColumnName("last_login_at");
            
            entity.Property(e => e.ProfilePictureUrl)
                .HasColumnName("profile_picture_url")
                .HasMaxLength(500);
            
            entity.Property(e => e.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("jsonb");

            // Indexes
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("ix_users_is_active");
            
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("ix_users_created_at");
            
            entity.HasIndex(e => e.LastLoginAt)
                .HasDatabaseName("ix_users_last_login_at");
        });

        // Configure ApplicationRole
        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("roles");
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();
            
            entity.Property(e => e.IsSystemRole)
                .HasColumnName("is_system_role")
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("ix_roles_is_active");
            
            entity.HasIndex(e => e.IsSystemRole)
                .HasDatabaseName("ix_roles_is_system_role");
        });

        // Configure Identity tables with snake_case naming
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("user_roles");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("user_claims");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("user_logins");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("user_tokens");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("role_claims");
        });
    }

    private void ConfigureOAuthEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OAuthClient>(entity =>
        {
            entity.ToTable("oauth_clients");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ClientId)
                .HasColumnName("client_id")
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.ClientName)
                .HasColumnName("client_name")
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.ClientSecret)
                .HasColumnName("client_secret")
                .HasMaxLength(500);
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);
            
            entity.Property(e => e.AllowedGrantTypes)
                .HasColumnName("allowed_grant_types")
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.AllowedScopes)
                .HasColumnName("allowed_scopes")
                .IsRequired()
                .HasMaxLength(1000);
            
            entity.Property(e => e.RedirectUris)
                .HasColumnName("redirect_uris")
                .IsRequired()
                .HasMaxLength(2000);
            
            entity.Property(e => e.PostLogoutRedirectUris)
                .HasColumnName("post_logout_redirect_uris")
                .HasMaxLength(2000);
            
            entity.Property(e => e.AllowedCorsOrigins)
                .HasColumnName("allowed_cors_origins")
                .HasMaxLength(2000);
            
            entity.Property(e => e.IsEnabled)
                .HasColumnName("is_enabled")
                .IsRequired();
            
            entity.Property(e => e.RequireConsent)
                .HasColumnName("require_consent")
                .IsRequired();
            
            entity.Property(e => e.RequirePkce)
                .HasColumnName("require_pkce")
                .IsRequired();
            
            entity.Property(e => e.AccessTokenLifetime)
                .HasColumnName("access_token_lifetime")
                .IsRequired();
            
            entity.Property(e => e.RefreshTokenLifetime)
                .HasColumnName("refresh_token_lifetime")
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            // Indexes
            entity.HasIndex(e => e.ClientId)
                .IsUnique()
                .HasDatabaseName("ix_oauth_clients_client_id");
            
            entity.HasIndex(e => e.IsEnabled)
                .HasDatabaseName("ix_oauth_clients_is_enabled");
        });

        modelBuilder.Entity<OAuthScope>(entity =>
        {
            entity.ToTable("oauth_scopes");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.DisplayName)
                .HasColumnName("display_name")
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);
            
            entity.Property(e => e.IsEnabled)
                .HasColumnName("is_enabled")
                .IsRequired();
            
            entity.Property(e => e.RequireConsent)
                .HasColumnName("require_consent")
                .IsRequired();
            
            entity.Property(e => e.IsEmphasized)
                .HasColumnName("is_emphasized")
                .IsRequired();
            
            entity.Property(e => e.IsSystemScope)
                .HasColumnName("is_system_scope")
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            // Indexes
            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("ix_oauth_scopes_name");
            
            entity.HasIndex(e => e.IsEnabled)
                .HasDatabaseName("ix_oauth_scopes_is_enabled");
            
            entity.HasIndex(e => e.IsSystemScope)
                .HasDatabaseName("ix_oauth_scopes_is_system_scope");
        });
    }
}
