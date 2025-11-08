#!/bin/bash

# EF Core Migration Helper Script
# Usage: ./migrate.sh [command] [environment] [migration_name]
# Examples:
#   ./migrate.sh update Local
#   ./migrate.sh add Development AddNewEntity
#   ./migrate.sh info Development

set -e

COMMAND=${1:-"help"}
ENVIRONMENT=${2:-"Local"}
MIGRATION_NAME=${3:-""}

INFRASTRUCTURE_PROJECT="src/AscendID_API.Infrastructure"
STARTUP_PROJECT="src/AscendID_API.WebAPI"

# Validate environment
if [[ ! "$ENVIRONMENT" =~ ^(Local|Development|Production)$ ]]; then
    echo "❌ Invalid environment: $ENVIRONMENT"
    echo "Valid environments: Local, Development, Production"
    exit 1
fi

# Set colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 AscendID API Migration Tool${NC}"
echo -e "${YELLOW}Environment: $ENVIRONMENT${NC}"
echo

case $COMMAND in
    "update")
        echo -e "${GREEN}📋 Updating database to latest migration...${NC}"
        ASPNETCORE_ENVIRONMENT=$ENVIRONMENT dotnet ef database update \
            --project $INFRASTRUCTURE_PROJECT \
            --startup-project $STARTUP_PROJECT
        echo -e "${GREEN}✅ Database update completed!${NC}"
        ;;
    
    "add")
        if [ -z "$MIGRATION_NAME" ]; then
            echo -e "${RED}❌ Migration name is required for 'add' command${NC}"
            echo "Usage: ./migrate.sh add $ENVIRONMENT <MigrationName>"
            exit 1
        fi
        echo -e "${GREEN}📝 Adding new migration: $MIGRATION_NAME${NC}"
        ASPNETCORE_ENVIRONMENT=$ENVIRONMENT dotnet ef migrations add "$MIGRATION_NAME" \
            --project $INFRASTRUCTURE_PROJECT \
            --startup-project $STARTUP_PROJECT
        echo -e "${GREEN}✅ Migration '$MIGRATION_NAME' created!${NC}"
        ;;
    
    "remove")
        echo -e "${YELLOW}🗑️  Removing last migration...${NC}"
        ASPNETCORE_ENVIRONMENT=$ENVIRONMENT dotnet ef migrations remove \
            --project $INFRASTRUCTURE_PROJECT \
            --startup-project $STARTUP_PROJECT
        echo -e "${GREEN}✅ Last migration removed!${NC}"
        ;;
    
    "drop")
        echo -e "${RED}⚠️  WARNING: This will drop the entire database!${NC}"
        read -p "Are you sure you want to drop the database? (y/N): " confirm
        if [[ $confirm =~ ^[Yy]$ ]]; then
            ASPNETCORE_ENVIRONMENT=$ENVIRONMENT dotnet ef database drop --force \
                --project $INFRASTRUCTURE_PROJECT \
                --startup-project $STARTUP_PROJECT
            echo -e "${GREEN}✅ Database dropped!${NC}"
        else
            echo -e "${YELLOW}⏸️  Operation cancelled${NC}"
        fi
        ;;
    
    "info")
        echo -e "${BLUE}ℹ️  Database context information:${NC}"
        ASPNETCORE_ENVIRONMENT=$ENVIRONMENT dotnet ef dbcontext info \
            --project $INFRASTRUCTURE_PROJECT \
            --startup-project $STARTUP_PROJECT
        ;;
    
    "list")
        echo -e "${BLUE}📋 Available migrations:${NC}"
        ASPNETCORE_ENVIRONMENT=$ENVIRONMENT dotnet ef migrations list \
            --project $INFRASTRUCTURE_PROJECT \
            --startup-project $STARTUP_PROJECT
        ;;
    
    "script")
        echo -e "${GREEN}📄 Generating SQL script...${NC}"
        ASPNETCORE_ENVIRONMENT=$ENVIRONMENT dotnet ef migrations script \
            --project $INFRASTRUCTURE_PROJECT \
            --startup-project $STARTUP_PROJECT \
            --output "migration-script-$(date +%Y%m%d_%H%M%S).sql"
        echo -e "${GREEN}✅ SQL script generated!${NC}"
        ;;
    
    "help"|*)
        echo -e "${BLUE}📖 Available commands:${NC}"
        echo
        echo -e "${GREEN}update${NC}    [environment]                - Apply all pending migrations"
        echo -e "${GREEN}add${NC}       [environment] [name]          - Create a new migration"
        echo -e "${GREEN}remove${NC}    [environment]                - Remove the last migration"
        echo -e "${GREEN}drop${NC}      [environment]                - Drop the database (with confirmation)"
        echo -e "${GREEN}info${NC}      [environment]                - Show database context info"
        echo -e "${GREEN}list${NC}      [environment]                - List all migrations"
        echo -e "${GREEN}script${NC}    [environment]                - Generate SQL migration script"
        echo
        echo -e "${YELLOW}Environments:${NC} Local, Development, Production"
        echo
        echo -e "${BLUE}Examples:${NC}"
        echo "  ./migrate.sh update Local"
        echo "  ./migrate.sh add Development AddUserProfiles"
        echo "  ./migrate.sh info Development"
        echo "  ./migrate.sh list Local"
        ;;
esac