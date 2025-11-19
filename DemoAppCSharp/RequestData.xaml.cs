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
    /// Логика взаимодействия для RequestData.xaml
    /// </summary>
    public partial class RequestData : Window
    {
        dbForDemo0202Entities db = new dbForDemo0202Entities();
        PartnerRequest partnerRequest;
        public RequestData(PartnerRequest partnerRequest1)
        {
            InitializeComponent();
            partnerRequest = partnerRequest1;

            //заполняю поля нужными данными из базы
            cb_partners.ItemsSource = db.Partner.ToList();
            cb_partnersType.ItemsSource = db.PartnerType.ToList();
            LB_products.ItemsSource = db.Product.ToList();

            //вызываю метод для заполнения полей данными парнтнера
            getPartnerData(partnerRequest, null);

            //делаю поля недоступными
            tb_email.IsEnabled = false;
            tb_directorFio.IsEnabled = false;
            tb_phoneNumber.IsEnabled = false;
            tb_rating.IsEnabled = false;
            tb_address.IsEnabled = false;
            tb_productName.IsEnabled = false;
            tb_directorFio.IsEnabled = false;

        }

        public void getPartnerData (PartnerRequest partnerRequest, Partner partner)
        {
            //заполняю поля партнера в соответствии с выбранной заявкой

            //нахожу в базе данных партнера
            //если передан параметр "заявка партнера", то беру данные о партнере оттуда
            if (partnerRequest != null)
            {
                var selectedPartner = db.Partner.FirstOrDefault(p => p.ID == partnerRequest.Partner1.ID);
                cb_partners.SelectedItem = selectedPartner;
                cb_partners.IsEnabled = false;

                var selectedPartnerType = db.PartnerType.FirstOrDefault(pt => pt.ID == partnerRequest.Partner1.PartnerType);
                cb_partnersType.SelectedItem = selectedPartnerType;
                cb_partnersType.IsEnabled = false;

                tb_directorFio.Text = partnerRequest.Partner1.Director;
                tb_email.Text = partnerRequest.Partner1.Email;
                tb_phoneNumber.Text = partnerRequest.Partner1.PhoneNumber;
                tb_rating.Text = Convert.ToString(partnerRequest.Partner1.Rating);
                tb_address.Text = partnerRequest.Partner1.Address;

                cb_productInRequest.ItemsSource = db.PartnerRequestList.Where(x => x.PartnerRequest == partnerRequest.ID).ToList();
            }
            else
            {
                var partnerInDB = db.Partner.FirstOrDefault(x => x.ID == partner.ID);

                cb_partnersType.SelectedItem = partnerInDB.PartnerType1;
                tb_directorFio.Text = partnerInDB.Director;
                tb_email.Text = partnerInDB.Email;
                tb_phoneNumber.Text = partnerInDB.PhoneNumber;
                tb_rating.Text = Convert.ToString(partnerInDB.Rating);
                tb_address.Text = partnerInDB.Address;
            }
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void cb_productInRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = cb_productInRequest.SelectedItem as PartnerRequestList;

            if (selectedProduct != null)
            {
                tb_quantity.Text = Convert.ToString(selectedProduct.Quantity);
                tb_productName.Text = selectedProduct.Product1.ProductName;
            }

        }

        private void btn_addProductInRequest_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = LB_products.SelectedItem as Product;

            if (selectedProduct != null)
            {
                if (partnerRequest != null)
                {
                    //проверяю, есть ли продукт в текущей заявке
                    var isSelectedProductAlreadyInList = db.PartnerRequestList.FirstOrDefault(x => x.PartnerRequest == partnerRequest.ID && x.Product == selectedProduct.ID);

                    if (isSelectedProductAlreadyInList != null)
                    {
                        MessageBox.Show("Выбранный продукт уже есть в заявке!", "Ошибка");
                        return;
                    }
                    else
                    {
                        //создаю новую запись в PartnerRequestList
                        PartnerRequestList newRequestItem = new PartnerRequestList()
                        {
                            PartnerRequest = partnerRequest.ID,
                            Product = selectedProduct.ID,
                            Quantity = 1 //устанавливаю количество по умолчанию = 1
                        };

                        //добавляю в бд
                        db.PartnerRequestList.Add(newRequestItem);
                        db.SaveChanges();

                        //обновляю комбобокс
                        cb_productInRequest.ItemsSource = db.PartnerRequestList.Where(x => x.PartnerRequest == partnerRequest.ID).ToList();

                        MessageBox.Show("Товар успешно добавлен в заявку!", "Успех");
                    }
                }
                else
                {
                    //создаю PartnerRequestList
                    PartnerRequestList newRequestItem = new PartnerRequestList()
                    {
                        Product = selectedProduct.ID,
                        Quantity = 1
                    };

                    //получаю текущие элементы из комбобокса
                    var currentItems = new List<PartnerRequestList>();

                    foreach (var item in cb_productInRequest.Items)
                    {
                        if (item is PartnerRequestList requestItem)
                        {
                            currentItems.Add(requestItem);
                        }
                    }

                    //добавляю новый товар в список
                    currentItems.Add(newRequestItem);

                    //обновляю комбобокс
                    cb_productInRequest.ItemsSource = currentItems;
                }
            }
            else
                MessageBox.Show("Выберите товар!", "Ошибка");
        }

        private void btn_saveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedProductInRequest = cb_productInRequest.SelectedItem as PartnerRequestList;

                if (selectedProductInRequest != null)
                {
                    var productInDB = db.PartnerRequestList.FirstOrDefault(x => x.ID == selectedProductInRequest.ID);

                    if (productInDB != null)
                    {
                        //проверяю количество на корректность
                        if (!tb_quantity.Text.Contains("-") && !tb_quantity.Text.Contains("."))
                        {
                            productInDB.Quantity = Convert.ToInt32(tb_quantity.Text);
                            db.SaveChanges();

                            MessageBox.Show("Количество товара успешно изменено!", "Успех");
                        }
                        else
                            MessageBox.Show("Введите верное число!", "Ошибка");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Проверьте заполненность данных!", "Ошибка");
            }
        }

        private void btn_delProduct_Click(object sender, RoutedEventArgs e)
        {
            //получаю выбранный товар
            var selectedRequestItem = cb_productInRequest.SelectedItem as PartnerRequestList;

            if (selectedRequestItem != null)
            {
                //нахожу и удаляю запись из БД
                var productInDb = db.PartnerRequestList.FirstOrDefault(x => x.ID == selectedRequestItem.ID);

                if (productInDb != null)
                {
                    db.PartnerRequestList.Remove(productInDb);
                    db.SaveChanges();

                    //обновляю комбобокс
                    cb_productInRequest.ItemsSource = db.PartnerRequestList.Where(x => x.PartnerRequest == partnerRequest.ID).ToList();

                    //очищаю поля
                    tb_quantity.Text = "";
                    tb_productName.Text = "";

                    MessageBox.Show("Товар удален из заявки!", "Успех");
                }
                else
                    MessageBox.Show("Выберите товар!", "Ошибка");
            }
            else
                MessageBox.Show("Выберите товар!", "Ошибка");
        }

        private void cb_partners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedPartner = cb_partners.SelectedItem as Partner;
                getPartnerData(null, selectedPartner);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + "", "Ошибка");
            }
        }

        
    }
}
