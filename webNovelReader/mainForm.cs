using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace webNovelReader
{
    public partial class mainForm : Form
    {
        int b, x = 0,dowl=0;
        bool download = false;
        string savepath;
        private FontStyle fs;
        private FontFamily fm;
        private float size = 8.5f;

        private string url = "https://www.ptwxz.com";

        private const int LocaleSystemDefault = 0x0800;
        private const int LcmapSimplifiedChinese = 0x02000000;
        private const int LcmapTraditionalChinese = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc,
                                              [Out] string lpDestStr, int cchDest);

        public static string ToSimplified(string argSource)
        {
            var t = new String(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapSimplifiedChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }

        public static string ToTraditional(string argSource)
        {
            var t = new String(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapTraditionalChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }


        public mainForm()
        {
            InitializeComponent();
        }



        private string second(string strHtml, int mode)
        {
            /*
        \ 標示下一個字元為特殊字元或文字，例如 n 對應字元 n，\n 則對應至換行字元。順序 \\ 對應 \，\( 則對應 (。 
        ^ 對應輸入的開頭。 
        $ 對應輸入的結尾。 
        * 對應前面的字元 0 次或更多次，例如 zo* 可對應到 z 或 zoo。 
        + 對應前面的字元一次或更多次，例如 zo+ 可對應到 zoo 而非 z。 
        ? 對應前面的字元 0 次或一次，例如 a?ve? 可對應到 never 中的 ve。 
        . 對應換行字元以外的任何單一字元。 
            */
            Regex objRegExp = new Regex("<(.|\n)+?>");
            // strHtml = strHtml.Replace("</li>", "\r\n");
            string strOutput = objRegExp.Replace(strHtml, "");
            strOutput = strOutput.Replace("\">", "");
            strOutput = strOutput.Replace("<", "&lt;");
            strOutput = strOutput.Replace(">", "&gt;");
            if (mode == 0)
            {
                strOutput = Regex.Replace(strOutput, "(\r\n)+", "\r\n");
                strOutput = strOutput.Replace("&nbsp;", "");
            }
            else if (mode == 1)
            {
                strOutput = strOutput.Replace("&nbsp;", "\r\n");
                strOutput = Regex.Replace(strOutput, "(\r\n)+", "\r\n\r\n");
            }

            return strOutput;
        }
        private string first(string strHtml2, int mode)
        {
            if (mode == 0)
            {
                Regex firstregex = new Regex("<!DOCTYPE(.|\n)+?\r\n<ul>\r\n");
                string strOutput2 = firstregex.Replace(strHtml2, "");

                Regex firstregex2 = new Regex("\r\n\\s</div>(.|\n)+?</html>");
                strOutput2 = firstregex2.Replace(strOutput2, "");
                // strOutput2 = strOutput2.Replace("</li>", "\r\n");
                //  strOutput2 = strOutput2.Replace("<", "&lt;");
                //  strOutput2 = strOutput2.Replace(">", "&gt;");

                return strOutput2;
            } else
            {

                Regex firstregex = new Regex("<head>(.|\n)+?</tr></table>");
                string strOutput2 = firstregex.Replace(strHtml2, "");

                Regex firstregex2 = new Regex("</div>(.|\n)+?</html>");
                strOutput2 = firstregex2.Replace(strOutput2, "");
                //  strOutput2 = firstregex2.Replace(strOutput2, "");
                // strOutput2 = strOutput2.Replace("</li>", "\r\n");
                //  strOutput2 = strOutput2.Replace("<", "&lt;");
                //  strOutput2 = strOutput2.Replace(">", "&gt;");

                return strOutput2;
            }
        }


        private void geturl(string uurl, int mode)
        {
            if (mode == 0)
            {
                Regex objRegExpurl = new Regex("\">(.|\n)+?</li>");
                string urlOutput = objRegExpurl.Replace(uurl, "");

                Regex objRegExpur2 = new Regex("<li>(.|\n)+?=\"");
                urlOutput = objRegExpur2.Replace(urlOutput, "");

                textBox2.Text = second(urlOutput, 0);

                string[] aryString = (!String.IsNullOrEmpty(textBox2.Text.Trim())) ? textBox2.Lines : null;
                chapterUrl.Items.AddRange(aryString);
                //  listBox2.Items.RemoveAt(0);
                //  while (listBox2.Items[Convert.ToInt16(listBox2.Items.Count.ToString()) - 1].ToString() == " " || listBox2.Items[Convert.ToInt16(listBox2.Items.Count.ToString()) - 1].ToString() == "")
                //      listBox2.Items.RemoveAt(Convert.ToInt16(listBox2.Items.Count.ToString()) - 1);
                textBox2.Clear();
            }
            else
            {
                Regex objRegExpurl = new Regex("\\.html(.|\n)+?</a>");
                string urlOutput = objRegExpurl.Replace(uurl, "");
                Regex objRegExpur2 = new Regex("<td(.|\n)+?info/");
                urlOutput = objRegExpur2.Replace(urlOutput, "");
                urlOutput = Regex.Replace(urlOutput, "(\\s)+", "\r\n");
                textBox2.Text = urlOutput;
                string[] aryString = (!String.IsNullOrEmpty(textBox2.Text.Trim())) ? textBox2.Lines : null;
                listBox4.Items.AddRange(aryString);
                textBox2.Clear();
            }
        }


        private void getnovel()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(webBrowser2.DocumentStream, Encoding.GetEncoding(webBrowser2.Document.Encoding));
            //  string temp;
            textBox1.Text = "";
            /*
            while (true)
            {
                temp = reader.ReadLine();
                if (temp == "</tr></table>")
                    break;
            }
            */
            textBox1.Text = reader.ReadToEnd();
            reader.Close();
        }

        private void getnovellist()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(webBrowser1.DocumentStream, Encoding.GetEncoding(webBrowser1.Document.Encoding));
            /*
            string temp;
            while (true)
            {
                temp = reader.ReadLine();
                if (temp == "<div class=\"list\">正文</div>")
                    break;
            }
            */
            textBox1.Text = reader.ReadToEnd();
        }
        private void searchresult()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(searchPage.DocumentStream, Encoding.GetEncoding(searchPage.Document.Encoding));
            /*
            string temp;
            while (true)
            {
                temp = reader.ReadLine();
                if (temp == "<div class=\"list\">正文</div>")
                    break;
            }
            */
            textBox2.Text = reader.ReadToEnd();
            reader.Close();
        }

        private void getsearchresult(string slist)
        {

            //  Regex firstregex = new Regex("<!DOCTYPE(.|\n)+?\\s</tr>\r\n\\s\\s\r\n");
            Regex firstregex = new Regex("<!DOCTYPE(.|\n)+?<tr>\r\n\\s+");
            string strOutput2 = firstregex.Replace(slist, "");
            //判斷<tr>數目取得搜尋結果量
            Regex firstregex2 = new Regex("</table>(.|\n)+?</html>");
            strOutput2 = firstregex2.Replace(strOutput2, "");

            Regex firstregex3 = new Regex("</td>(.|\n)+?</tr>");
            strOutput2 = firstregex3.Replace(strOutput2, "");

            strOutput2 = strOutput2.Replace("<tr>\r\n", "");

            geturl(strOutput2, 1);

            Regex firstregex4 = new Regex("<td(.|\n)+?\\.html\">");
            strOutput2 = firstregex4.Replace(strOutput2, "");

            strOutput2 = Regex.Replace(strOutput2, "(\\s)+", "\r\n");
            strOutput2 = strOutput2.Replace("</a>", "");


            //  strOutput2 = strOutput2.Replace("<", "&lt;");
            //  strOutput2 = strOutput2.Replace(">", "&gt;");

            textBox2.Text = strOutput2;
            string[] aryString = (!String.IsNullOrEmpty(textBox2.Text.Trim())) ? textBox2.Lines : null;
            resultList.Items.AddRange(aryString);
            //textBox2.Clear();
        }
        private void button1_Click(object sender, EventArgs e)
        {

            // string[] aryString = (!String.IsNullOrEmpty(textBox2.Text.Trim())) ? textBox2.Lines : null;
            /*
            b++;
            string a = listBox2.Items[b].ToString();
            webBrowser2.Navigate(webBrowser1.Url.ToString() + a);
            */
            chapterList.SelectedIndex++;

        }




        private void Form1_Load(object sender, EventArgs e)
        {
            fs = textBox1.Font.Style;
            fm = new FontFamily(textBox1.Font.Name);
            //webBrowser1.Navigate("http://www.ptwxz.com/html/8/8313/");
            searchPage.Navigate(url + "/modules/article/search.php");
            string path = @"d:\aaa\a.ini";
            if (File.Exists(path))
            {
                FileStream fsFile = new FileStream(path, FileMode.Open);
                StreamReader reader = new StreamReader(fsFile);
                while (reader.Peek() >= 0)
                {
                    bookcase.Items.Add(reader.ReadLine());
                    listBox6.Items.Add(reader.ReadLine());
                }
                reader.Close();
            }

        }



        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            getnovellist();
            textBox1.Text = first(textBox1.Text, 0);
            geturl(textBox1.Text, 0);
            textBox1.Text = second(textBox1.Text, 0);
            //  string[] aryString = textBox1.Lines;
            //  listBox1.Items.AddRange(aryString);
            string[] aryString = (!String.IsNullOrEmpty(textBox1.Text.Trim())) ? textBox1.Lines : null;
            chapterList.Items.AddRange(aryString);
            //   MessageBox.Show("."+listBox1.Items[Convert.ToInt16(listBox1.Items.Count.ToString())-1].ToString()+"."); 
            while (chapterList.Items[Convert.ToInt16(chapterList.Items.Count.ToString()) - 1].ToString() == " " || chapterList.Items[Convert.ToInt16(chapterList.Items.Count.ToString()) - 1].ToString() == "")
                chapterList.Items.RemoveAt(Convert.ToInt16(chapterList.Items.Count.ToString()) - 1);
            //     while (listBox1.Items[Convert.ToInt16(listBox1.Items.Count.ToString())-1].ToString() == "")
            //        listBox1.Items.RemoveAt(Convert.ToInt16(listBox1.Items.Count.ToString())-1);

            nextBtn.Enabled = false;
            textBox1.Text = "";
            button7.Enabled = true;
            string a = webBrowser1.Url.ToString();
            string[] getpic = a.Split('/');
            coverPic.ImageLocation = (url + "/files/article/image/" + getpic[4] + "/" + getpic[5]+"/" + getpic[5] + "s.jpg");


            Cursor = Cursors.Default;
        }

        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            getnovel();
            textBox1.Text = first(textBox1.Text, 1);
            textBox1.Text = second(textBox1.Text, 1);
            if (radioButton2.Checked == true)
                textBox1.Text = ToTraditional(textBox1.Text);


            if (nextBtn.Enabled == false)
                nextBtn.Enabled = true;
            Cursor = Cursors.Default;
            if (download == true)
            {
                FileStream fsFile = new FileStream(savepath+@"log" + dowl + ".txt", FileMode.OpenOrCreate);               
                StreamWriter swWriter = new StreamWriter(fsFile);
                swWriter.WriteLine(textBox1.Text);
                //swWriter.WriteLine("It is now {0}", DateTime.Now.ToLongDateString());
                swWriter.Close();

                progressBar1.Value += 5;

                chapterList.SelectedIndex++;
                if (chapterList.SelectedIndex == chapterList.Items.Count)
                {
                    download = false;
                    dowl = 0;
                    UseWaitCursor = false;
                    MessageBox.Show("Done.");
                }
                dowl++;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            b = chapterList.SelectedIndex;
            string a = chapterUrl.Items[b].ToString();
            webBrowser2.Navigate(webBrowser1.Url.ToString() + a);
            toolStripStatusLabel4.Text = chapterList.SelectedItem.ToString();
            Cursor = Cursors.Default;
        }


        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (searchInput.Text == "")
            {
                MessageBox.Show("請輸入搜索內容", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Cursor = Cursors.WaitCursor;
                var search = searchPage.Document.GetElementsByTagName("input");
                search[4].SetAttribute("value", searchInput.Text);
                //webBrowser3.Document.GetElementById("searchkey").SetAttribute("value", textBox3.Text);
                search[6].InvokeMember("click");
                
            }
        }

        private void webBrowser3_NewWindow(object sender, CancelEventArgs e)
        {

            e.Cancel = true;
            string newurl = searchPage.StatusText;
            MessageBox.Show(newurl);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            size += 1;
            textBox1.Font = new Font(fm, size, fs);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            size -= 1;
            textBox1.Font = new Font(fm, size, fs);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
                textBox1.Text = ToTraditional(textBox1.Text);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                textBox1.Text = ToSimplified(textBox1.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (bookcase.SelectedIndex >= 0)
            {

                Cursor = Cursors.WaitCursor;
                string a = listBox6.Items[bookcase.SelectedIndex].ToString();
                string[] savefolder = a.Split('/');
                savepath = @"d:\aaa\" + savefolder[1]+@"\";
                System.IO.Directory.CreateDirectory(savepath);
                UseWaitCursor = true;
                download = true;
                chapterList.SelectedIndex = 0;
                progressBar1.Value = 0;
                //  for (int i = 0; i < listBox1.Items.Count; i++)
                //  {

                // download = true;

                //寫入數據
                /*
                while (true)
                {
                    if (download == false)
                    {
                        swWriter.WriteLine(textBox1.Text);
                        break;
                    }
                    Thread.Sleep(500);
                }
                */


                //   }

                Cursor = Cursors.Default;
            }
            else
                MessageBox.Show("選擇收藏的書籍才能緩存喔!","提醒",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            chapterList.Items.Clear();
            chapterUrl.Items.Clear();
            b = resultList.SelectedIndex;
            string a = listBox4.Items[b].ToString();
            webBrowser1.Navigate(url + "/html/" + a);
            toolStripStatusLabel2.Text = resultList.SelectedItem.ToString();
            Cursor = Cursors.Default;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (resultList.SelectedIndex >= 0)
            {
                bookcase.Items.Add(resultList.SelectedItem.ToString());
                listBox6.Items.Add(listBox4.Items[b].ToString());
                string a = listBox4.Items[b].ToString();
                FileStream fsFile = new FileStream(@"d:\aaa\a.ini", FileMode.Append);
                StreamWriter swWriter = new StreamWriter(fsFile);
                swWriter.WriteLine(resultList.SelectedItem.ToString() + "\r\n" + a);
                //swWriter.WriteLine("It is now {0}", DateTime.Now.ToLongDateString());
                swWriter.Close();
                MessageBox.Show("收藏成功囉!", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("請選擇書籍", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            chapterList.Items.Clear();
            chapterUrl.Items.Clear();
            if (bookcase.SelectedIndex >= 0)
            {
                b = bookcase.SelectedIndex;
                string a = listBox6.Items[b].ToString();
                webBrowser1.Navigate(url + "/html/" + a);
                toolStripStatusLabel2.Text = bookcase.SelectedItem.ToString();
                toolStripStatusLabel4.Text = "";
                tabControl1.SelectedIndex = 1;

            }
            Cursor = Cursors.Default;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bookcase.SelectedIndex >= 0)
            {
                DialogResult myResult = MessageBox.Show("確定要刪除嗎?", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (myResult == DialogResult.Yes)
            {
               
                    listBox6.Items.RemoveAt(bookcase.SelectedIndex);
                    bookcase.Items.RemoveAt(bookcase.SelectedIndex);
                    string path = @"d:\aaa\a.ini";
                 //   if (File.Exists(path))
                 //       File.Delete(path);
                    FileStream fsFile = new FileStream(path, FileMode.Create);
                    StreamWriter swWriter = new StreamWriter(fsFile);
                    for (int i = 0; i < bookcase.Items.Count; i++)
                    {
                        swWriter.WriteLine(bookcase.Items[i].ToString() + "\r\n" + listBox6.Items[i].ToString());
                    }
                    //swWriter.WriteLine("It is now {0}", DateTime.Now.ToLongDateString());
                    swWriter.Close();
                }
                
            }
            else
                MessageBox.Show("請選擇書籍","提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void textBox3_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchBtn.Focus();
                button3_Click(sender, e);
            }
        }



        private void webBrowser3_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            x++;
            if (x == 2)
            {
                
                resultList.Items.Clear();
                listBox4.Items.Clear();
                searchresult();
                getsearchresult(textBox2.Text);
                
                x = 0;
                searchPage.Navigate(url + "/modules/article/search.php");
                Cursor = Cursors.Default;
            }
        }
    }
    }

