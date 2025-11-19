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
    /// Логика взаимодействия для MethodFour.xaml
    /// </summary>
    public partial class MethodFour : Window
    {
        dbForDemo0202Entities db = new dbForDemo0202Entities();
        public MethodFour()
        {
            InitializeComponent();

            //заполняю комбобоксы
            cb_productType.ItemsSource = db.ProductType.ToList();
            cb_materialType.ItemsSource = db.MaterialType.ToList();
        }

        private void btn_count_Click(object sender, RoutedEventArgs e)
        {
            //получаю выбранные типы материала и товара
            ProductType selectedPType = cb_productType.SelectedItem as ProductType;
            MaterialType selectedMType = cb_materialType.SelectedItem as MaterialType;

            try
            {
                double result = Method(selectedPType.ID, selectedMType.ID, Convert.ToInt32(tb_quantityInWarehouse.Text), Convert.ToInt32(tb_needQuantity.Text), 
                    Convert.ToDouble(tb_param1.Text), Convert.ToDouble(tb_param2.Text));

                //обработка результата, равного -1
                if (result == -1)
                {
                    MessageBox.Show("Проверьте заполненность полей!", "Ошибка");
                    return;
                }
                MessageBox.Show("Результат: " + result, "Результат");
            }
            catch
            {
                MessageBox.Show("Введите корректные поля!");
            }
        }

        private int Method(int productTypeID, int materialTypeID, int quantityInWarehouse, int needQuantity, double param1, double param2)
        {
            //нахожу необходимые в базе переменные
            ProductType productType = db.ProductType.FirstOrDefault(x => x.ID == productTypeID);
            MaterialType materialType = db.MaterialType.FirstOrDefault(x => x.ID == materialTypeID);

            if (productType == null || materialType == null || quantityInWarehouse <= 0 || needQuantity <= 0 || param1 <= 0 || param2 <= 0)
            {
                return -1;
            }

            // Проверяем, что коэффициенты не равны null
            if (productType.CoefficentOfLostness == null || materialType.CoefficentOfLost == null)
            {
                MessageBox.Show("Коэффициенты потерь не заданы для выбранного типа продукта или материала.", "Ошибка");
                return -1; 
            }

            double? coeffOfLostness = productType.CoefficentOfLostness;
            double? coeffOfLost = materialType.CoefficentOfLost;
            double? quantityForOne = param2 * param1 * coeffOfLostness;
            double? quantityOfLost = (quantityForOne * coeffOfLost) + quantityForOne;
            double? productTorelease = needQuantity - quantityInWarehouse;

            double? howMuchMaterial = quantityOfLost * productTorelease;

            //округляю до бОльшего
            int result = (int)Math.Round(Convert.ToDouble(howMuchMaterial));

            return result;
        }
    }
}
