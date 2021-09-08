using System.ComponentModel;

namespace MyLights.Models
{
    public interface ILight
    {
        double Brightness { get; }
        HSV Color { get; }
        double ColorTemp { get; }
        string Id { get; }
        LightMode Mode { get; }
        bool Power { get; }

        Scene Scene { get; }

        event PropertyChangedEventHandler PropertyChanged;

        void SetBrightness(double value, bool immediate = false);
        void SetColor(HSV value, bool immediate = false);
        void SetColorTemp(double value, bool immediate = false);
        void SetMode(LightMode value, bool immediate = false);
        void SetPower(bool value, bool immediate = false);

        void SetScene(Scene value, bool immediate = false);
    }
}