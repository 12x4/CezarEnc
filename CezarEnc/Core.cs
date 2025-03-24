using System;
//using System.Collections.Generic;
using System.Linq;
//using System.Security.Policy;
using System.Text;
//using System.Threading.Tasks;
using System.Numerics;
using System.CodeDom.Compiler;
using System.Security.Cryptography;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Drawing.Printing;

namespace CezarEnc
{
    internal class Core
    {

        internal String text { get; set; }
        internal String encryp_Text { get; set; }

        internal String status_message { get; set; }
        public bool error_status {get; set;}

        public String lang_gl {get; set;}
        
        private BigInteger key = new BigInteger();
        public int supposed_key { get; set; }

        private String dictionary_Ru = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";
        private String dictionary_En = "abcdefghijklmnopqrstuvwxyz0123456789";
        private String numbers = "0123456789";

        private int len_Dictionary_Ru = 43;
        private int len_Dictionary_En = 36;

        private HackCez hack = new HackCez();

        //Инициализация
        public Core()
        {
            error_status = false;
            lang_gl = "";
        }

        //шифровка
        public void encrypted()
        {
            String lang = getLangText(text);
            StringBuilder ecn_text = new StringBuilder();

            String dict = null;
            int dict_len = 0;

            //очистка текста
            //text = cleanText(text);

            //установление статуса работы анализом текста
            set_status_through_lang(lang, ref dict, ref dict_len);

            //если в ходе анализа или в другом месте вылезла ошибка то прекращаем работу
            if (error_status) return;

            BigInteger indx;

            foreach (char symb in text.ToLower())
            {

                if (dict.Contains(symb))
                {
                    indx = ((BigInteger)dict.IndexOf(symb) + key) % (BigInteger)dict_len;
                    if (indx < 0)
                    {
                        indx += (BigInteger)dict_len;
                    }
                    ecn_text.Append(dict[(int)indx]);
                }

                else
                {
                    ecn_text.Append(symb);
                }

            }

            //устанавливаем статус работы
            status_message = "Успешно";
            error_status = false;

            encryp_Text = ecn_text.ToString();
        }

        //расшифровка
        public void decrypted()
        {
            String lang = getLangText(encryp_Text);
            StringBuilder decry_text = new StringBuilder();

            String dict = null;
            int dict_len = 0;

            //очистка текста
            //encryp_Text = cleanText(encryp_Text);

            //установление статуса работы анализом текста
            set_status_through_lang(lang, ref dict, ref dict_len);

            //если в ходе анализа или в другом месте вылезла ошибка то прекращаем работу
            if (error_status) return;

            BigInteger indx;

            foreach (char symb in encryp_Text.ToLower())
            {

                if (dict.Contains(symb))
                {
                    indx = ((BigInteger)dict.IndexOf(symb) - key) % (BigInteger)dict_len;
                    if (indx < 0)
                    {
                        indx += (BigInteger)dict_len;
                    }

                    decry_text.Append(dict[(int)indx]);
                }

                else
                {
                    decry_text.Append(symb);
                }

            }

            //устанавливаем статус работы
            status_message = "Успешно";
            error_status = false;

            text = decry_text.ToString();
        }

        //дешифровка
        public String hack_txt()
        {
            String lang = getLangText(encryp_Text);
            StringBuilder decry_text = new StringBuilder();

            String dict = null;
            int dict_len = 0;

            //очистка текста
            //encryp_Text = cleanText(encryp_Text);

            //установление статуса работы анализом текста
            set_status_through_lang(lang, ref dict, ref dict_len);

            //если в ходе анализа или в другом месте вылезла ошибка то прекращаем работу
            if (error_status) return null;

            BigInteger indx;

            if (lang == "num")
            {
                lang = lang_gl;
            }

            supposed_key = hack.hacking(encryp_Text, lang);

            foreach (char symb in encryp_Text.ToLower())
            {

                if (dict.Contains(symb))
                {
                    indx = ((BigInteger)dict.IndexOf(symb) - supposed_key) % (BigInteger)dict_len;
                    if (indx < 0)
                    {
                        indx += (BigInteger)dict_len;
                    }

                    decry_text.Append(dict[(int)indx]);
                }

                else
                {
                    decry_text.Append(symb);
                }

            }

            //устанавливаем статус работы
            status_message = "Успешно";
            error_status = false;

            return decry_text.ToString();

        }

        //Очситка текста
        private String cleanText(String text)
        {
            StringBuilder new_s = new StringBuilder();
            String del_symbl = "";

            foreach(char symb in text)
            {
                if (!del_symbl.Contains(symb))
                {
                    new_s.Append(symb);
                }
            }

            return new_s.ToString();
        }

        //метод для определения языка текста
        private String getLangText(String text)
        {
            String lang = "-1";

            int eng_symbols = 0;
            int ru_symbols = 0;

            int numbers_n = 0;

            int len_text = text.Length;
            
            foreach (char symb in text.ToLower())
            {
                if (numbers.Contains(symb))
                {
                    numbers_n += 1;
                    continue;
                }

                if (dictionary_En.Contains(symb))
                {
                    eng_symbols += 1;
                    continue;
                }

                if (dictionary_Ru.Contains(symb))
                {
                    ru_symbols += 1;
                    continue;
                }

                lang = "0";
                return lang;

            }

            if (eng_symbols > 0 && ru_symbols == 0)
            {
                lang = "eng";
            }

            if (ru_symbols > 0 && eng_symbols == 0)
            {
                lang = "ru";
            }

            if (ru_symbols == 0 && eng_symbols == 0 && numbers_n > 0)
            {
                lang = "num";
            }

            return lang;
        }

        //установка статуса анализом языка
        private void set_status_through_lang(String lang, ref String Xdict, ref int Xdict_len)
        {
            if (lang_gl == "")
            {
                status_message = "Выберите язык";
                error_status = true;
                return;
            }

            switch (lang)
            {
                case "ru":
                    Xdict = dictionary_Ru;
                    Xdict_len = len_Dictionary_Ru;
                    status_message = "Успешно";
                    error_status = false;
                    break;

                case "eng":
                    Xdict = dictionary_En;
                    Xdict_len = len_Dictionary_En;
                    status_message = "Успешно";
                    error_status = false;
                    break;

                case "0":
                    status_message = "Язык неопределенн или есть лишние символы";
                    error_status = true;
                    break;

                case "-1":
                    status_message = "Нету текста";
                    error_status = true;
                    break;

                case "num":
                    set_status_through_lang(lang_gl, ref Xdict, ref Xdict_len);
                    return;
            }

            if(!error_status)
            {
                if (lang_gl != lang)
                {
                    status_message = "Выбран неправильный язык";
                    error_status = true;
                    return;
                }
            }
        }

        //Проверка и установка ключа
        public void set_key(String _key)
        {
            try 
            {   
                if (_key.Contains(' '))
                {
                    status_message = "Неправильный ключ\n Ключ это целочисленное число!!!";
                    error_status = true;
                    return;
                }
                

                key = BigInteger.Parse(_key);
                status_message = "Успешно";
                error_status = false;
            }
            catch
            {
                status_message = "Неправильный ключ\n Ключ это целочисленное число!!!";
                error_status = true;
            }
        }

        public String getDictEnc()
        {
            StringBuilder new_str = new StringBuilder();
            foreach(var per in hack.data.Reverse())
            {
                new_str.Append($"{per.Key} - {Math.Round(per.Value, 4)}%" + Environment.NewLine);
            }

            return new_str.ToString();
        }
    }

    internal class HackCez
    {
        internal int supposed_key { get; set; }

        private Dictionary<char, double> data_ru = new Dictionary<char, double>();
        private Dictionary<char, double> data_eng = new Dictionary<char, double>();

        internal Dictionary<char, double> data  = new Dictionary<char, double>();

        private String dictionary_Ru = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";
        private String dictionary_Eng = "abcdefghijklmnopqrstuvwxyz0123456789";

        private String ru_path_txt = "C:\\Users\\Камиль Тухватуллин\\source\\repos\\CezarEnc\\CezarEnc\\dataTxt\\dataRu.txt";
        private String eng_path_txt = "C:\\Users\\Камиль Тухватуллин\\source\\repos\\CezarEnc\\CezarEnc\\dataTxt\\dataEng.txt";

        internal HackCez()
        {
            setDictThAnlzFile(ru_path_txt, ref data_ru);
            setDictThAnlzFile(eng_path_txt, ref data_eng);
        }

        // Устанавливаем в словарь данные текста
        internal void setDictThAnlzFile(String file, ref Dictionary<char, double> data)
        {
            StreamReader _file = new StreamReader(file);
            String [] s = new String [2];
            
            while (!_file.EndOfStream)
            {
                s = _file.ReadLine().Split();
                data.Add(Convert.ToChar(s[0]), Convert.ToDouble(s[1]));
            }

        }

        // Устанавливаем в словарь данные текста
        internal void setDictThAnlzText(String text, ref Dictionary<char, double> data)
        {
            double len_text = text.Length;
            Dictionary<char, int> data_lok = new Dictionary<char, int>();

            foreach (char symbl in text.ToLower())
            {
                if (data_lok.ContainsKey(symbl))
                {
                    data_lok[symbl] += 1;
                }
                else
                {
                    data_lok.Add(symbl, 1);
                }
            }

            data.Clear();

            foreach (var poz in data_lok)
            {
                data.Add(poz.Key, (poz.Value / len_text) * 100);
            }

            data = data.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        // Установка предполагаемого ключа
        internal void setSuppKey(ref Dictionary<char, double> data_enc, String lang)
        {
            Dictionary<char, double> data = null;

            switch (lang)
            {
                case "ru":
                    data = data_ru;
                    break;

                case "eng":
                    data = data_eng;
                    break;
            }
            // Находим максимальное значение в каждом словаре
            double max1 = data.Values.Max();
            double max2 = data_enc.Values.Max();

            // Находим ключи с максимальными значениями в каждом словаре
            char keysWithMax1 = getKeyThrouVal(ref data, max1);
            char keysWithMax2 = getKeyThrouVal(ref data_enc, max2);

            switch(lang)
            {
                case "ru":
                    supposed_key = dictionary_Ru.IndexOf(Convert.ToChar(keysWithMax2)) - dictionary_Ru.IndexOf(Convert.ToChar(keysWithMax1));
                    break;

                case "eng":
                    supposed_key = dictionary_Eng.IndexOf(Convert.ToChar(keysWithMax2)) - dictionary_Eng.IndexOf(Convert.ToChar(keysWithMax1));
                    break;
            }

        }

        internal int hacking(String text, String lang)
        {
            setDictThAnlzText(text, ref data);
            setSuppKey(ref data, lang);

            return supposed_key;
        }

        // возвращает ключ по элементу
        private char getKeyThrouVal(ref Dictionary<char, double> dict, double val)
        {            
            foreach(var per in dict)
            {
                if (per.Value == val) { return per.Key; }
            }

            return ' ';
        }
    }
}
