using System;
//using System.Collections.Generic;
using System.Linq;
//using System.Security.Policy;
using System.Text;
//using System.Threading.Tasks;
using System.Numerics;
using System.CodeDom.Compiler;
using System.Security.Cryptography;

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

        private String dictionary_Ru = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";
        private String dictionary_En = "abcdefghijklmnopqrstuvwxyz0123456789";
        private String numbers = "0123456789";

        private int len_Dictionary_Ru = 43;
        private int len_Dictionary_En = 36;

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
            set_status_through_lang(getLangText(encryp_Text), ref dict, ref dict_len);

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
                    break;

                case "eng":
                    Xdict = dictionary_En;
                    Xdict_len = len_Dictionary_En;
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
                foreach(char symb in _key)
                {
                    if (symb == ' ')
                    {
                        status_message = "Неправильный ключ\n Ключ это целочисленное число!!!";
                        error_status = true;
                        return;
                    }
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
    }
}
