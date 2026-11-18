using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class CataloguePage : UraniumContentPage
{
    public CataloguePage()
        : this(new CatalogueViewModel())
    {
    }

    public CataloguePage(CatalogueViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        var contentWidth = Math.Min(720, Math.Max(0, width));

        if (contentWidth > 0 && Math.Abs(CatalogueContent.WidthRequest - contentWidth) > 0.5)
        {
            CatalogueContent.WidthRequest = contentWidth;
        }
    }
}
