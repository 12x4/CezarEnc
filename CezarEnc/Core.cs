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

        internal String error_message { get; set; }
        public bool error_status { get; set; }

        public String lang_gl { get; set; }

        private String key = null;
        public int supposed_key { get; set; }

        private String dictionary_Ru = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";
        private String dictionary_En = "abcdefghijklmnopqrstuvwxyz0123456789";
        private String numbers = "0123456789";

        private const int len_Dictionary_Ru = 43;
        private const int len_Dictionary_En = 36;

        private char[][] arr_ru_2x2 = new char[len_Dictionary_Ru][];
        private char[][] arr_eng_2x2 = new char[len_Dictionary_En][];

        //Инициализация
        public Core()
        {
            error_status = false;
            lang_gl = "";

            for (int i = 0; i < len_Dictionary_Ru; i++) { arr_ru_2x2[i] = new char[len_Dictionary_Ru]; }
            for (int i = 0; i < len_Dictionary_En; i++) { arr_eng_2x2[i] = new char[len_Dictionary_En]; }


            for (int i = 0; i < len_Dictionary_Ru; i++)
            {
                for (int g = 0; g < len_Dictionary_Ru; g++)
                {

                    arr_ru_2x2[i][g] = dictionary_Ru[(i + g + 1) % len_Dictionary_Ru];
                }
            }

            for (int i = 0; i < len_Dictionary_En; i++)
            {
                for (int g = 0; g < len_Dictionary_En; g++)
                {

                    arr_eng_2x2[i][g] = dictionary_En[(i + g + 1) % len_Dictionary_En];
                }
            }

        }

        //шифровка
        public void encrypted()
        {
            String lang = getLangText(text);

            String key_new = keyExtensn(key, text.Length);
            StringBuilder ecn_text = new StringBuilder();

            char [][] dict = null;
            int dict_len = 0;
            String _dict = null;

            //Проверяем корректность выбранного языка
            isCorrectLangInstld(lang);
            //если в ходе анализа или в другом месте вылезла ошибка то прекращаем работу
            if (error_status) { return; }

            //установление статус работы
            setStatusThrghLang(lang);
            //если в ходе анализа или в другом месте вылезла ошибка то прекращаем работу
            if (error_status) { return; }

            //устанавливаем значения в dict и dict_len и _dict
            setArrThroughLang(ref dict);
            setDictThroughLang(ref _dict);
            setLenThroughLang(ref dict_len);

            for (int i = 0; i < key_new.Length; i++)
            {

                int x, y;

                x = _dict.IndexOf(key_new[i]);
                y = _dict.IndexOf(text[i]);

                ecn_text.Append(dict[x][y]);

            }

            //устанавливаем статус работы
            error_status = false;

            encryp_Text = ecn_text.ToString();
        }

        //расшифровка
        public void decrypted()
        {
            String lang = getLangText(encryp_Text);

            String key_new = keyExtensn(key, encryp_Text.Length);
            StringBuilder decr_text = new StringBuilder();

            char[][] dict = null;
            int dict_len = 0;
            String _dict = null;

            //Проверяем корректность выбранного языка
            isCorrectLangInstld(lang);
            //если в ходе анализа или в другом месте вылезла ошибка то прекращаем работу
            if (error_status) { return; }

            //установление статус работы
            setStatusThrghLang(lang);
            //если в ходе анализа или в другом месте вылезла ошибка то прекращаем работу
            if (error_status) { return; }

            //устанавливаем значения в dict и dict_len и _dict
            setArrThroughLang(ref dict);
            setDictThroughLang(ref _dict);
            setLenThroughLang(ref dict_len);

            for (int i = 0; i < key_new.Length; i++)
            {

                int x, y;

                x = _dict.IndexOf(key_new[i]);
                y = _dict.IndexOf(encryp_Text[i]);

                int shift = y - (x + 1);

                if (shift < 0)
                {
                    shift += dict_len;
                }

                decr_text.Append(dict[x][shift]);

            }

            //устанавливаем статус работы
            error_status = false;

            encryp_Text = decr_text.ToString();
        }

        //Очситка текста
        public String cleanText(String text)
        {
            StringBuilder new_s = new StringBuilder();
            String _symbl = "";

            if (lang_gl == "")
            {
                error_message = "Выберите язык";
                error_status = true;
                return text;
            }

            switch (lang_gl)
            {
                case "ru":
                    _symbl = dictionary_Ru;
                    break;

                case "eng":
                    _symbl = dictionary_En;
                    break;
            }

            foreach(char symb in text.ToLower())
            {
                if (_symbl.Contains(symb))
                {
                    new_s.Append(symb);
                }
            }

            error_status = false;

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
        private void setStatusThrghLang(String lang)
        {

            switch (lang)
            {
                case "ru":
                    error_status = false;
                    break;

                case "eng":
                    error_status = false;
                    break;

                case "0":
                    error_message = "Язык неопределенн или есть лишние символы";
                    error_status = true;
                    break;

                case "-1":
                    error_message = "Нету текста";
                    error_status = true;
                    break;

                case "num":
                    setStatusThrghLang(lang_gl);
                    return;
            }
        }
        
        //
        private void setArrThroughLang(ref char[][] Xdict)
        {

            switch (lang_gl)
            {
                case "ru":
                    Xdict = arr_ru_2x2;
                    error_status = false;
                    break;

                case "eng":
                    Xdict = arr_eng_2x2;
                    error_status = false;
                    break;
            }

        }

        //
        private void setDictThroughLang(ref String Xdict)
        {
            switch (lang_gl)
            {
                case "ru":
                    Xdict = dictionary_Ru;
                    error_status = false;
                    break;

                case "eng":
                    Xdict = dictionary_En;
                    error_status = false;
                    break;
            }

        }

        //
        private void setLenThroughLang(ref int Xlen)
        {

            switch(lang_gl)
            {
                case "ru":
                    Xlen = len_Dictionary_Ru;
                    break;

                case "eng":
                    Xlen = len_Dictionary_En;
                    break;
            }

        }

        // Проверка на корректный выбор языка 
        private bool isCorrectLangInstld(String lang)
        {

            if (lang_gl == "")
            {
                error_message = "Выберите язык";
                error_status = true;
                return false;
            }

            if (lang == "num")
            {
                return true;
            }

            if (lang_gl != lang)
            {
                error_message = "Выбран неправильный язык";
                error_status = true;
                return false;
            }

            error_status = false;
            return true; 
        }

        //Проверка и установка ключа
        public void set_key(String _key)
        {
            String lang_key = getLangText(_key);

            // Проверяем на корректность выбранного языка
            if (!isCorrectLangInstld(lang_key)) { return; }


            switch (lang_key)
            {
                case "ru":
                    error_status = false;
                    break;

                case "eng":
                    error_status = false;
                    break;

                case "num":
                    lang_key = lang_gl;
                    error_status = false;
                    break;

                default:
                    error_message = "Неправильный ключ";
                    error_status = true;
                    break;
            }

            key = _key.ToLower();

        }

        //Удлинение ключча до необходимой длины
        private String keyExtensn(String org_key, int len_text)
        {
            StringBuilder key = new StringBuilder();

            int count = len_text / org_key.Length;

            for (int i = 0; i < count; i++) { key.Append(org_key); }
            for (int i = 0; i < len_text % org_key.Length; i++) { key.Append(org_key[i]); }

            return key.ToString();
        }

    }

}
