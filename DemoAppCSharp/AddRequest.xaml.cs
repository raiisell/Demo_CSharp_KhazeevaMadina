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
using System.Windows.Shapes;

namespace DemoAppCSharp
{
    /// <summary>
    /// Логика взаимодействия для AddRequest.xaml
    /// </summary>
    public partial class AddRequest : Window
    {
        dbForDemo0202Entities db = new dbForDemo0202Entities();
        //лист для хранения товаров в заявке
        private List<PartnerRequestList> productsInRequest = new List<PartnerRequestList>();
        public AddRequest()
        {
            InitializeComponent();

            //заполняю поля нужными данными из базы
            cb_partners.ItemsSource = db.Partner.ToList();
            cb_partnersType.ItemsSource = db.PartnerType.ToList();
            LB_products.ItemsSource = db.Product.ToList();

            //делаю поля недоступными
            cb_partnersType.IsEnabled = false;
            tb_email.IsEnabled = false;
            tb_directorFio.IsEnabled = false;
            tb_phoneNumber.IsEnabled = false;
            tb_rating.IsEnabled = false;
            tb_address.IsEnabled = false;
            tb_productName.IsEnabled = false;
            tb_directorFio.IsEnabled = false;

            cb_productInRequest.ItemsSource = productsInRequest;
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void cb_partners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //нахожу партнера в базе
            var selectedPartner = cb_partners.SelectedItem as Partner;
            var partnerInDB = db.Partner.FirstOrDefault(x => x.ID == selectedPartner.ID);

            //заполняю данные о партнере
            cb_partnersType.SelectedItem = partnerInDB.PartnerType1;
            tb_directorFio.Text = partnerInDB.Director;
            tb_email.Text = partnerInDB.Email;
            tb_phoneNumber.Text = partnerInDB.PhoneNumber;
            tb_rating.Text = Convert.ToString(partnerInDB.Rating);
            tb_address.Text = partnerInDB.Address;
        }

        private void btn_addProductInRequest_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = LB_products.SelectedItem as Product;

            if (selectedProduct != null)
            {
                var isProductInRequest = productsInRequest.FirstOrDefault(x => x.Product == selectedProduct.ID);
                //проверяю, нет ли уже такого продукта в списке
                if (isProductInRequest != null)
                {
                    MessageBox.Show("Этот товар уже добавлен в заявку.", "Внимание");
                    return;
                }

                //создаю новую запись для списка товаров заявки
                PartnerRequestList newRequestItem = new PartnerRequestList
                {
                    Product = selectedProduct.ID,
                    Product1 = selectedProduct,
                    Quantity = 1
                };

                productsInRequest.Add(newRequestItem);

                cb_productInRequest.ItemsSource = null;

                //обновляю отображение комбобокса
                cb_productInRequest.ItemsSource = productsInRequest; 

                MessageBox.Show($"Товар '{selectedProduct.ProductName}' добавлен в заявку.", "Успех");
            }
            else
            {
                MessageBox.Show("Выберите товар!", "Ошибка");
            }
        }

        private void btn_addRequest_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли партнер
            var selectedPartner = cb_partners.SelectedItem as Partner;
            if (selectedPartner == null)
            {
                MessageBox.Show("Выберите партнера.", "Ошибка");
                return;
            }

            //есть ли товары в заявке
            if (productsInRequest.Count == 0)
            {
                MessageBox.Show("Добавьте товар в заявку.", "Ошибка");
                return;
            }

            try
            {
                //создаю новую заявку
                PartnerRequest partnerRequest = new PartnerRequest
                {
                    Partner = selectedPartner.ID
                };

                db.PartnerRequest.Add(partnerRequest);
                db.SaveChanges();

                foreach (var item in productsInRequest)
                {
                    //создаю запись списка товаров
                    PartnerRequestList partnerRequestList = new PartnerRequestList
                    {
                        PartnerRequest = partnerRequest.ID, 
                        Product = item.Product,
                        Quantity = item.Quantity 
                    };
                    db.PartnerRequestList.Add(partnerRequestList);
                }

                db.SaveChanges();

                MessageBox.Show("Заявка успешно добавлена!", "Успех");

                productsInRequest.Clear();
                cb_productInRequest.ItemsSource = productsInRequest;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void cb_productInRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProductInRequest = cb_productInRequest.SelectedItem as PartnerRequestList;

            if (selectedProductInRequest != null)
            {
                //заполняю поля информацией о выбранном товаре
                tb_productName.Text = selectedProductInRequest.Product1.ProductName;
                tb_quantity.Text = Convert.ToString(selectedProductInRequest.Quantity);
            }
            else
            {
                tb_productName.Text = "";
                tb_quantity.Text = "";
            }
        }

        private void btn_saveQuantity_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный товар в комбобоксе "Состав заявки"
            var selectedRequestItem = cb_productInRequest.SelectedItem as PartnerRequestList;

            if (selectedRequestItem == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (Convert.ToInt32(tb_quantity.Text) <= 0 || tb_quantity.Text.Contains("-") || tb_quantity.Text.Contains("."))
            {
                MessageBox.Show("Количество должно быть положительным числом.", "Ошибка");
                return;
            }

            //обновляю количество
            selectedRequestItem.Quantity = Convert.ToInt32(tb_quantity.Text);
            tb_quantity.Text = selectedRequestItem.Quantity.ToString();

            MessageBox.Show("Количество успешно изменено!", "Успех");
        }
    }
}
