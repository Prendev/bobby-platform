namespace QvaDev.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactoringDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StratDealingArbs", "SignalDiffInPip", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "SignalStepInPip", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "TargetInPip", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "ShiftInPip", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.StratDealingArbs", "PipSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StratDealingArbs", "PipSize", c => c.Double(nullable: false));
            AlterColumn("dbo.StratDealingArbs", "ShiftInPip", c => c.Double());
            AlterColumn("dbo.StratDealingArbs", "TargetInPip", c => c.Double(nullable: false));
            AlterColumn("dbo.StratDealingArbs", "SignalStepInPip", c => c.Double(nullable: false));
            AlterColumn("dbo.StratDealingArbs", "SignalDiffInPip", c => c.Double(nullable: false));
        }
    }
}
