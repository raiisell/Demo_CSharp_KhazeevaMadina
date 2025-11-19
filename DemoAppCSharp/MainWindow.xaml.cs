using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoAppCSharp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        dbForDemo0202Entities db = new dbForDemo0202Entities();
        List<PartnerRequest> requests = new List<PartnerRequest>();
        public MainWindow()
        {
            InitializeComponent();

            requests = db.PartnerRequest.ToList();

            try
            {
                requests = db.PartnerRequest.ToList();

                foreach (var request in requests)
                {
                    //создаю границу
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(1);

                    //создаю wrappanel
                    WrapPanel wrapPanel = new WrapPanel();
                    WrapPanel wrapL = new WrapPanel();
                    Grid wrapR = new Grid();

                    wrapL.Margin = new Thickness(20, 10, 0, 0);
                    wrapR.VerticalAlignment = VerticalAlignment.Center;

                    //создаю текстовые поля для левого wrappanel
                    TextBlock type_name = new TextBlock();
                    TextBlock partner_name = new TextBlock();
                    TextBlock phoneNumber = new TextBlock();
                    TextBlock rating = new TextBlock();
                    TextBlock price_unit = new TextBlock();

                    //для правого wrappanel
                    TextBlock price = new TextBlock();

                    //прописываю необходимые настройки для wrappanel - ширину и высоту
                    wrapL.Orientation = Orientation.Vertical;

                    wrapPanel.Width = 700;
                    wrapPanel.Height = 100;

                    wrapL.Width = 350;
                    wrapL.Height = 100;

                    wrapR.Width = 300;
                    wrapR.Height = 100;

                    TextBlock Edit = new TextBlock();
                    Edit.Text = "Изменить";
                    Edit.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0C4882"));

                    Edit.Uid = Convert.ToString(request.ID);
                    Edit.MouseDown += new MouseButtonEventHandler(LB_request_SelectionChanged);

                    wrapPanel.Children.Add(wrapL);
                    wrapPanel.Children.Add(wrapR);

                    wrapL.Children.Add(type_name);
                    wrapL.Children.Add(partner_name);
                    wrapL.Children.Add(phoneNumber);
                    wrapL.Children.Add(rating);
                    wrapL.Children.Add(price_unit);
                    wrapL.Children.Add(Edit);

                    //для того, чтобы у каждого блока была граница, добавляю все wrappanel в borderbrush
                    border.Child = wrapPanel;

                    wrapR.Children.Add(price);

                    type_name.FontSize = 14;

                    type_name.Text = $"{request.Partner1.PartnerType1} | {request.Partner1.PartnerName}";
                    partner_name.Text = $"{request.Partner1.Address}";
                    phoneNumber.Text = $"{request.Partner1.PhoneNumber}";
                    rating.Text = $"{request.Partner1.Rating}";


                    price.FontSize = 14;
                    price.VerticalAlignment = VerticalAlignment.Center;
                    price.HorizontalAlignment = HorizontalAlignment.Center;

                    //рассчет стоимости
                    double purchase = 0.00;

                    var productsInRequest = db.PartnerRequestList.Where(x => x.PartnerRequest == request.ID).ToList();

                    foreach (var product in productsInRequest)
                    {
                        purchase += Convert.ToDouble(product.Quantity * product.Product1.MinCostForPartner);
                    }

                    price.Text = $"Стоимость партии: {purchase:F2} р.";

                    LB_request.Items.Add(border);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex, "Ошибка");
            }
        }
        private void LB_request_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mousePosition = Mouse.GetPosition(this);
            IInputElement element = InputHitTest(mousePosition);
            string request = (element as FrameworkElement)?.Uid;

            int requestID = Convert.ToInt32(request);

            var selectedRequest = db.PartnerRequest.FirstOrDefault(x => x.ID == requestID);

            if (selectedRequest != null)
            {
                RequestData materialsData = new RequestData(selectedRequest);
                this.Close();
                materialsData.Show();
            }
        }

        private void btn_addRequest_Click(object sender, RoutedEventArgs e)
        {
            AddRequest addRequest = new AddRequest();
            addRequest.Show();
            this.Close();
        }

        private void btn_count_Click(object sender, RoutedEventArgs e)
        {
            MethodFour methodFour = new MethodFour();
            methodFour.Show();
            this.Close();
        }
    }
}
