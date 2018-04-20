
namespace ASD
{

    public class CarpentersBench : System.MarshalByRefObject
    {

    /// <summary>
    /// Metoda pomocnicza - wymagana przez system
    /// </summary>
    public int Cut(int length, int width, int [,] elements, out Cut cuts)
        {
        (int length, int width, int price)[] _elements = new (int length, int width, int price)[elements.GetLength(0)];
        for (int i=0 ; i<_elements.Length ; ++i )
            {
            _elements[i].length = elements[i,0];
            _elements[i].width = elements[i,1];
            _elements[i].price = elements[i,2];
            }
        return Cut((length,width), _elements, out cuts);
        }

    /// <summary>
    /// Wyznaczanie optymalnego sposobu pocięcia płyty
    /// </summary>
    /// <param name="sheet">Rozmiary płyty</param>
    /// <param name="elements">Tablica zawierająca informacje o wymiarach i wartości przydatnych elementów</param>
    /// <param name="cuts">Opis cięć prowadzących do uzyskania optymalnego rozwiązania</param>
    /// <returns>Maksymalna sumaryczna wartość wszystkich uzyskanych w wyniku cięcia elementów</returns>
        public int Cut((int length, int width) sheet, (int length, int width, int price)[] elements, out Cut cuts)
        {
            Cut[,] cutTable = new Cut[sheet.length + 1, sheet.width + 1];
            foreach (var element in elements)
            {
                if (element.length <= sheet.length && element.width <= sheet.width)
                {
                    if (cutTable[element.length, element.width] == null)
                    {
                        cutTable[element.length, element.width] = new Cut(element.length, element.width, element.price);
                    }
                    else if (cutTable[element.length, element.width].price < element.price)
                    {
                        cutTable[element.length, element.width].price = element.price;
                    }


                }
            }
            for (int i = 1; i <= sheet.length; i++)
            {
                for (int j = 1; j <= sheet.width; j++)
                {
                    if (cutTable[i, j] == null)
                    {
                        cutTable[i, j] = new Cut(i, j, 0);
                    }
                    for (int k = 1; k <= i - 1; k++)
                    {
                        if (cutTable[k, j].price + cutTable[i - k, j].price > cutTable[i, j].price)
                        {
                            cutTable[i, j].price = cutTable[k, j].price + cutTable[i - k, j].price;
                            cutTable[i, j].vertical = false;
                            cutTable[i, j].n = k;
                            cutTable[i, j].topleft = cutTable[k, j];
                            cutTable[i, j].bottomright = cutTable[i - k, j];
                        }
                    }
                    for (int k = 1; k <= j - 1; k++)
                    {
                        if (cutTable[i, k].price + cutTable[i, j - k].price > cutTable[i, j].price)
                        {
                            cutTable[i, j].price = cutTable[i, k].price + cutTable[i, j - k].price;
                            cutTable[i, j].vertical = true;
                            cutTable[i, j].n = k;
                            cutTable[i, j].topleft = cutTable[i, k];
                            cutTable[i, j].bottomright = cutTable[i, j - k];
                        }
                    }
                }
            }
            cuts = cutTable[sheet.length, sheet.width];
            return cutTable[sheet.length, sheet.width].price;
        }

    }

}
