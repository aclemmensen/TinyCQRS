// <auto-generated />
namespace TinyCQRS.ReadModel.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    public sealed partial class FixCrawlTimestamps : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(FixCrawlTimestamps));
        
        string IMigrationMetadata.Id
        {
            get { return "201302071857067_FixCrawlTimestamps"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}