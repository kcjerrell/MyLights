using MyLights.Models;

namespace MyLights.ViewModels
{
    public class SceneSetter
    {
        public SceneSetter(BulbRef bulbRef)
        {
            this.BulbRef = bulbRef;
        }

        public void Set()
        {
            Light?.SetMode(Mode);
            Light?.SetPower(Power);

            if (Mode == "white")
            {

            }
            else if (Mode == "color")
            {
                Light?.SetColor(Color);
            }
        }

        public bool IsLive { get; set; }
        public Light Light { get; set; }
        public HSV Color { get; set; }
        public bool Power { get; set; }
        public BulbRef BulbRef { get; set; }

        private bool _isColorMode;
        public bool IsColorMode
        {
            get
            {
                return _isColorMode;
            }
            set
            {
                if (value != _isColorMode)
                {
                    _isColorMode = value;
                    if (IsLive)
                        Light?.SetMode(Mode);
                }
            }
        }
   
        public string Mode
        {
            get
            {
                if (IsColorMode)
                    return "color";
                else
                    return "white";
            }
            set
            {
                string low = value.ToLower();
                if (low == "color")
                    IsColorMode = true;
                else if (low == "white")
                    IsColorMode = false;
            }
        }

        public double H
        {
            get => Color.H;
            set => SetColor(value, Color.S, Color.V);
        }

        public double S
        {
            get => Color.S;
            set => SetColor(Color.H, value, Color.V);
        }

        public double V
        {
            get => Color.H;
            set => SetColor(Color.H, Color.S, value);
        }

        private void SetColor(double h, double s, double v)
        {
            Color = new HSV(h, s, v);
            if (IsLive)
                Light?.SetColor(Color);
        }

        internal void FindBulb()
        {
            var lightBridge = App.Current.Locator.LightBridge;
            if (lightBridge.TryFindBulb(BulbRef, out Light light))
            {
                Light = light;
            }
        }
    }
}