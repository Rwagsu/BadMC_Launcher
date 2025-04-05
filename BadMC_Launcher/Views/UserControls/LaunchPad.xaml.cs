using BadMC_Launcher.ViewModels.UserControls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;
public sealed partial class LaunchPad : UserControl {
    public LaunchPad() {
        this.InitializeComponent();
        DataContext = new LaunchPadViewModel();
    }
}
