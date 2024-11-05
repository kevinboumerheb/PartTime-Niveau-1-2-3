namespace Kev.Xrm.Plugins.AppCode
{
    public class PluginStage
    {
        // Context.Stage constants
        public const int PreValidation = 10;
        public const int PreOperation = 20;
        public const int PostOperation = 40;

        // Non supporté pour les plugins CRM 2011
        // public const int PostValidation = 50;
    }
}
