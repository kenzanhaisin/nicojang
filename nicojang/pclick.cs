using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace nicojang
{
    public partial class Form1 : Form
    {

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            mt.WaitOne();

            switch (stage)
            {
                case "設定":

                    if(jibunid == m_userid)
                    {
                        if (new Rectangle(151, 51, 398, 98).Contains(pictureBox1.PointToClient(MousePosition)))
                        {//タグ
                            for (int i = 0; i < 20; i++)
                            {
                                if (new Rectangle(i * 20 + 151, 51, 18, 98).Contains(pictureBox1.PointToClient(MousePosition)))
                                {
                                    oto_zokuerabi.Play();
                                    if (sentakuzoku == i)
                                    {
                                        sentakuzoku = -1;
                                        textBoxcharzoku.Visible = false;
                                    }
                                    else
                                    {
                                        sentakuzoku = i;
                                        textBoxcharzoku.MaxLength = 6;
                                        textBoxcharzoku.Visible = true;
                                        textBoxcharzoku.Focus();
                                    }
                                    break;
                                }
                            }
                        }
                        else if (new Rectangle(51, 151, 98, 398).Contains(pictureBox1.PointToClient(MousePosition)))
                        {//キャラ
                            for (int j = 0; j < 10; j++)
                            {
                                if (new Rectangle(51, j * 40 + 151, 98, 38).Contains(pictureBox1.PointToClient(MousePosition)))
                                {
                                    oto_zokuerabi.Play();
                                    if (sentakuchar == j)
                                    {
                                        sentakuchar = -1;
                                        textBoxcharzoku.Visible = false;
                                    }
                                    else
                                    {
                                        sentakuchar = j;
                                        textBoxcharzoku.MaxLength = 32767;
                                        textBoxcharzoku.Visible = true;
                                        textBoxcharzoku.Focus();
                                    }
                                    break;
                                }
                            }
                        }
                        else if (new Rectangle(151, 151, 398, 398).Contains(pictureBox1.PointToClient(MousePosition)))
                        {//タグ値
                            for (int i = 0; i < 20; i++)
                            {
                                for (int j = 0; j < 9; j++)
                                {
                                    if (new Rectangle(i * 20 + 151, j * 40 + 151, 18, 38).Contains(pictureBox1.PointToClient(MousePosition)))
                                    {
                                        oto_zokuerabi.Play();

                                        if (p_setteihyo[i, j])
                                        {
                                            p_setteihyo[i, j] = false;
                                        }
                                        else
                                        {
                                            p_setteihyo[i, j] = true;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                        else if (zokuyomi.Contains(pictureBox1.PointToClient(MousePosition)))
                        {//タグ読取
                            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "nicojang")))
                            {
                                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\nicojang");
                                Properties.Settings.Default.zensetteipath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "nicojang");
                                Properties.Settings.Default.Save();

                                openFileDialog1.InitialDirectory = Properties.Settings.Default.zensetteipath;
                            }

                            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                            {//復元
                                using (StreamReader sr = new StreamReader(openFileDialog1.OpenFile()))
                                {
                                    string line = "";
                                    int kazoe = 0;
                                    while ((line = sr.ReadLine()) != null)
                                    {
                                        if (kazoe < 20)
                                        {
                                            p_zokusei[kazoe] = line;
                                        }
                                        else if (kazoe < 30)
                                        {
                                            p_charaid[kazoe - 20] = line;
                                        }
                                        else if (kazoe < 40)
                                        {
                                            p_charaimei[kazoe - 30] = line;
                                        }
                                        else if (kazoe < 50)
                                        {
                                            p_charagazouid[kazoe - 40] = line;
                                            try
                                            {
                                                req = (HttpWebRequest)(WebRequest.Create(gazouseiki + p_charagazouid[kazoe - 40]));
                                                req.CookieContainer = m_cc;//取得済みのクッキーコンテナ

                                                g = Graphics.FromImage(p_charagazou28[kazoe - 40]);
                                                g.PixelOffsetMode = PixelOffsetMode.Half;
                                                g.DrawImage(new Bitmap(req.GetResponse().GetResponseStream()), 0, 0, 28, 28);
                                            }
                                            catch
                                            {
                                                p_charagazouid[kazoe - 40] = "";
                                                p_charaimei[kazoe - 40] = "";
                                                p_charagazou28[kazoe - 40] = Properties.Resources.noimage28;
                                            }
                                        }
                                        else if (kazoe < 59)
                                        {
                                            string[] stArrayData = line.Split(',');
                                            for (int i = 0; i < 20; i++)
                                            {
                                                if (stArrayData[i] == "1")
                                                {
                                                    p_setteihyo[i, kazoe - 50] = true;
                                                }
                                                else
                                                {
                                                    p_setteihyo[i, kazoe - 50] = false;
                                                }
                                            }
                                        }
                                        kazoe++;
                                    }
                                }

                                Properties.Settings.Default.zensetteipath = openFileDialog1.InitialDirectory;
                                Properties.Settings.Default.Save();
                            }
                        }
                        else if (zokukaki.Contains(pictureBox1.PointToClient(MousePosition)))
                        {//設定保存
                            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "nicojang")))
                            {
                                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\nicojang");
                                Properties.Settings.Default.zensetteipath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "nicojang");
                                Properties.Settings.Default.Save();

                                saveFileDialog1.InitialDirectory = Properties.Settings.Default.zensetteipath;
                            }

                            setteihozon();
                        }
                        else if (zokukaku.Contains(pictureBox1.PointToClient(MousePosition)))
                        {//確定
                            oto_kettei.Play();

                            string er = "";
                            for (int i = 0; i < 20; i++)
                            {
                                if (p_zokusei[i] == "")
                                {
                                    for (int j = 0; j < 9; j++)
                                    {
                                        p_setteihyo[i, j] = false;
                                    }
                                }
                            }

                            for (int j = 0; j < 10; j++)
                            {
                                if (p_charaid[j] == "")
                                {
                                    er += j.ToString() + "番目のキャラクターを決定して下さい\r\n";
                                }
                            }

                            for (int i = 0; i < 20; i++)
                            {
                                if (p_zokusei[i] != "")
                                {
                                    int kazu = 0;
                                    for (int j = 0; j < 9; j++)
                                    {
                                        if (p_setteihyo[i, j])
                                        {
                                            kazu++;
                                        }
                                    }

                                    if (kazu < 2)
                                    {
                                        er += "「" + p_zokusei[i] + "」の選択を2つ以上にして下さい\r\n";
                                    }
                                }
                            }

                            if (er == "")
                            {
                                if (MessageBox.Show("設定を保存しますか？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                                {
                                    setteihozon();
                                }

                                stage = "面子";

                                Thread thsyokisosin = new Thread(setteisosin);
                                thsyokisosin.IsBackground = true;
                                thsyokisosin.Start();                                
                            }
                            else
                            {
                                MessageBox.Show(er);
                            }
                        }
                    }
                    break;

                case "面子":
                    if (jibunid == m_userid)
                    {
                        if (new Rectangle(400, 250, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)) || new Rectangle(250, 400, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)) || new Rectangle(100, 250, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)) || new Rectangle(250, 100, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                        {
                            if (listBox1.SelectedIndex > -1)
                            {
                                oto_zokuerabi.Play();

                                if (listBox1.SelectedIndex == 0)
                                {
                                    if (new Rectangle(400, 250, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                    {//右
                                        h_sankaid[0] = "-1";
                                        h_sankamei[0] = "人工無能東";
                                        h_sankagazou[0] = new Bitmap(Properties.Resources.noimage);
                                    }
                                    else if (new Rectangle(250, 100, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                    {//上
                                        h_sankaid[1] = "-1";
                                        h_sankamei[1] = "人工無能北";
                                        h_sankagazou[1] = new Bitmap(Properties.Resources.noimage);
                                    }
                                    else if (new Rectangle(100, 250, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                    {//左
                                        h_sankaid[2] = "-1";
                                        h_sankamei[2] = "人工無能西";
                                        h_sankagazou[2] = new Bitmap(Properties.Resources.noimage);
                                    }
                                    else if (new Rectangle(250, 400, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                    {//下
                                        h_sankaid[3] = "-1";
                                        h_sankamei[3] = "人工無能南";
                                        h_sankagazou[3] = new Bitmap(Properties.Resources.noimage);
                                    } 
                                }
                                else if (mentuchoufukuhantei(s_sankasyaid[listBox1.SelectedIndex]))
                                {
                                    try
                                    {
                                        if (s_sankasyaid[listBox1.SelectedIndex] != "")
                                        {
                                            req = (HttpWebRequest)(WebRequest.Create(gazouseiki + s_sankasyagazouid[listBox1.SelectedIndex]));
                                            req.CookieContainer = m_cc;//取得済みのクッキーコンテナ
                                            
                                            if (new Rectangle(400, 250, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                            {//右
                                                h_sankaid[0] = s_sankasyaid[listBox1.SelectedIndex];
                                                h_sankamei[0] = (string)listBox1.SelectedItem;
                                                h_sankagazou[0] = new Bitmap(req.GetResponse().GetResponseStream());
                                            }                                           
                                            else if (new Rectangle(250, 100, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                            {//上
                                                h_sankaid[1] = s_sankasyaid[listBox1.SelectedIndex];
                                                h_sankamei[1] = (string)listBox1.SelectedItem;
                                                h_sankagazou[1] = new Bitmap(req.GetResponse().GetResponseStream());
                                            }
                                            else if (new Rectangle(100, 250, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                            {//左
                                                h_sankaid[2] = s_sankasyaid[listBox1.SelectedIndex];
                                                h_sankamei[2] = (string)listBox1.SelectedItem;
                                                h_sankagazou[2] = new Bitmap(req.GetResponse().GetResponseStream());
                                            }
                                            else if (new Rectangle(250, 400, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                                            {//下
                                                h_sankaid[3] = s_sankasyaid[listBox1.SelectedIndex];
                                                h_sankamei[3] = (string)listBox1.SelectedItem;
                                                h_sankagazou[3] = new Bitmap(req.GetResponse().GetResponseStream());
                                            }                                                                                        
                                        }
                                        else
                                        {
                                            //184用の代入
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("既に選ばれています");
                                }
                            }
                            else
                            {
                                MessageBox.Show("右側で雀士を選んでください");
                            }
                        }
                        else if (new Rectangle(250, 250, 100, 100).Contains(pictureBox1.PointToClient(MousePosition)))
                        {
                            if (mentukauteihantei())
                            {
                                saiketu = false;
                                oto_saion.Play();
                                stage = "親決";

                                Thread thsyokisosin = new Thread(mentusosin);
                                thsyokisosin.IsBackground = true;
                                thsyokisosin.Start();                        
                            }
                            else
                            {
                                MessageBox.Show("全員決めて下さい");
                            }
                        }
                    }
                    break;

                case "対局":
                    if (jiseki > -1)
                    {
                        if (ponreacherea.Contains(pictureBox1.PointToClient(MousePosition)))
                        {//ポン
                            if (ponflag[jiseki])
                            {
                                ponflag[jiseki] = false;
                                comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, "##ポン," + jiseki.ToString() + "," + m_vpos));
                                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count+1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka("ポン," + jiseki.ToString()) + "</chat>");
                            }
                        }
                        else if (nonerea.Contains(pictureBox1.PointToClient(MousePosition)))
                        {//パス
                            if (ponflag[jiseki])
                            {
                                ponflag[jiseki] = false;
                                comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, "##ノン," + jiseki.ToString() + "," + m_vpos));
                                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka("ノン," + jiseki.ToString()) + "</chat>");
                            }
                        }
                        else if(new Rectangle(420, pictureBox1.Height - haitate, haiyoko, haitate).Contains(pictureBox1.PointToClient(MousePosition)))
                        {//捨牌８
                            if (jiseki == mawariban && tehai[jiseki][8] != 100 && !agariflag[jiseki] && tehai[jiseki][8] != 100)
                            {
                                comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, "##捨牌,8," + jiseki.ToString() + "," + m_vpos));
                                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka("捨牌,8" + "," + mawariban.ToString() + "," + Environment.TickCount.ToString()) + "</chat>");
                                haisutesyori(8);
                                break;
                            }
                        }
                        else
                        {//捨牌i
                            if (jiseki == mawariban && (tehai[jiseki][8] != 100 || ponken[jiseki]) && !agariflag[jiseki])
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    if (new Rectangle(i * haiyoko + haitate * 2, pictureBox1.Height - haitate, haiyoko, haitate).Contains(pictureBox1.PointToClient(MousePosition)) && tehai[jiseki][i] != 100)
                                    {
                                        comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, "##捨牌," + i.ToString() + "," + jiseki.ToString() + "," + m_vpos));
                                        //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka("捨牌," + i.ToString() + "," + mawariban.ToString() + "," + Environment.TickCount.ToString()) + "</chat>");
                                        haisutesyori(i);                                        
                                        break;
                                    }

                                }
                            }
                        }                    
                    }
                    else
                    {
                        if (miginameerea.Contains(pictureBox1.PointToClient(MousePosition)))
                        {
                            nagame = migi;
                            hougakuire();
                        }
                        else if (uenamearea.Contains(pictureBox1.PointToClient(MousePosition)))
                        {
                            nagame = ue;
                            hougakuire();
                        }
                        else if (hidarinamearea.Contains(pictureBox1.PointToClient(MousePosition)))
                        {
                            nagame = hidari;
                            hougakuire();
                        }
                        else if (sitanameerea.Contains(pictureBox1.PointToClient(MousePosition)))
                        {
                            nagame = sita;
                            hougakuire();
                        }
                    }

                    break;
            }

            mt.ReleaseMutex();
        }

        private bool mentuchoufukuhantei(string naid)
        {
            for (int i = 0; i < 4; i++)
            {
                if (h_sankaid[i] == naid)
                {
                    return false;
                }
            }

            return true;
        }

        private bool mentukauteihantei()
        {
            for (int i = 0; i < 4; i++)
            {
                if (h_sankaid[i] == "")
                {
                    return false;
                }
            }

            return true;
        }

        private void setteihozon()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string savedate = "";
                for (int i = 0; i < 20; i++)
                {
                    savedate += p_zokusei[i] + "\r\n";
                }

                for (int j = 0; j < 10; j++)
                {
                    savedate += p_charaid[j] + "\r\n";
                }

                for (int j = 0; j < 10; j++)
                {
                    savedate += p_charaimei[j] + "\r\n";
                }

                for (int j = 0; j < 10; j++)
                {
                    savedate += p_charagazouid[j] + "\r\n";
                }

                for (int j = 0; j < 9; j++)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (p_setteihyo[i, j])
                        {
                            savedate += "1,";
                        }
                        else
                        {
                            savedate += "0,";
                        }
                    }

                    savedate += "\r\n";
                    savedate.Replace(",\r\n", "\r\n");
                }

                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    sw.Write(savedate);
                }

                Properties.Settings.Default.zensetteipath = saveFileDialog1.InitialDirectory;
                Properties.Settings.Default.Save();
            }
        }

        private void setteisosin()
        {
            string stg = "##属設,";
            for (int i = 0; i < 20; i++)
            {
                stg += p_zokusei[i] + ",";
            }

            Invoke(new dldl(delegate
            {
               mt.WaitOne();
               syucom(stg);
               //comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, stg));
               //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + stg + "</chat>");
               mt.ReleaseMutex();
            }));

            stg = "##牌設,";
            for (int j = 0; j < 10; j++)
            {
                stg += p_charaid[j] + ",";
            }

            Invoke(new dldl(delegate
            {
               mt.WaitOne();
               syucom(stg);
               //comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, stg));
                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + stg + "</chat>");
               mt.ReleaseMutex();
            }));

            stg = "##値設,";
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (p_setteihyo[i, j])
                    {
                        stg += "1,";
                    }
                    else
                    {
                        stg += "0,";
                    }                    
                }
            }

            Invoke(new dldl(delegate
            {
               mt.WaitOne();
               syucom(stg);
               //comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, stg));
                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + stg + "</chat>");
               mt.ReleaseMutex();
            }));
        }

        private void haidukuri()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    hai[i, j] = new Bitmap(38, 52);
                    g = Graphics.FromImage(hai[i, j]);
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                    g.DrawImage(Properties.Resources.haimae3852, 0, 0, 38, 52);
                    g.FillRectangle(namanusicolor[i], new Rectangle(norisiro, norisiro, haiyoko - (norisiro * 2), haitate - (norisiro * 2)));
                    g.DrawImage(p_charagazou28[i], new Rectangle(norisiro + 1, norisiro + 1, 28, 28));
                    g.DrawString((j + 1).ToString(), new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), System.Drawing.Brushes.White, new Rectangle(norisiro, 33, 30, 15), sfnaka);
                }
            }

            hai_all = new Bitmap(38, 52);
            g = Graphics.FromImage(hai_all);
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.DrawImage(Properties.Resources.haimae3852, 0, 0, 38, 52);
            g.FillRectangle(namanusicolor[9], new Rectangle(norisiro, norisiro, haiyoko - (norisiro * 2), haitate - (norisiro * 2)));
            g.DrawImage(p_charagazou28[9], new Rectangle(norisiro + 1, norisiro + 1, 28, 28));
            g.DrawString("0", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), System.Drawing.Brushes.White, new Rectangle(norisiro, 33, 30, 15), sfnaka);

        }

        private void mentusosin()
        {
            //席戻し
            jiseki = -1;
            for (int jj = 0; jj < 4; jj++)
            {
                if (jibunid == h_sankaid[jj])
                {
                    jiseki = jj;
                    nagame = jj;
                }
            }

            //面子
            string stg = "##面子,";
            for (int i = 0; i < 4; i++)
            {
                stg += h_sankaid[i] + ",";
            }

            Invoke(new dldl(delegate
            {
                mt.WaitOne();
                syucom(stg);
                //comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, stg));
                mt.ReleaseMutex();
            }));

            sai1 = new Random(new Random(Environment.TickCount).Next(100)).Next(6);
            sai2 = new Random(Environment.TickCount).Next(6);

            Thread.Sleep(1000);
            
            saiketu = true;

            oya = (sai1 + sai2) % 4;
            mawariban = oya;
            firstoya = oya;
            hougakuire();

            stg = "##サイ,";
            stg += sai1.ToString() + "," + sai2.ToString();

            Thread.Sleep(4000);                        

            Invoke(new dldl(delegate
            {
                mt.WaitOne();
                syucom(stg);
                //comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, stg));
                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count+1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka(stg) + "</chat>");
                mt.ReleaseMutex();
            }));

            Thread.Sleep(4000);
            haipai();
        }
      
        private void haipai()
        {
            List<int> kariban = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    kariban.Add((i * 10) + j);
                }
            }
            kariban.Add(90);

            int midasi = 0;
            int kariire = 0;
            for (int i = 0; i < 82; i++)
            {
                kariire = new Random(Environment.TickCount + midasi).Next(kariban.Count);
                haijun[i] = kariban[kariire];
                midasi += kariire;
                kariban.RemoveAt(kariire);
            }

            //118文字
            string sstg = "##配牌";
            for (int i = 0; i < 82; i++)
            {
                sstg += "," + haijun[i].ToString();
            }

            Invoke(new dldl(delegate
            {
                mt.WaitOne();
                syucom(sstg);
                //comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, sstg));
                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka(sstg) + "</chat>");
                mt.ReleaseMutex();
            }));

            /*sstg = "##牌２";
            for (int i = 27; i < 54; i++)
            {
                sstg += "," + haijun[i].ToString();
            }

            comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, sstg));
            Invoke(new dldl(delegate
            {
                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka(sstg) + "</chat>");
            }));

            sstg = "##牌３";
            for (int i = 54; i < 82; i++)
            {
                sstg += "," + haijun[i].ToString();
            }

            comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, sstg));
            Invoke(new dldl(delegate
            {
                //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"24704235\" premium=\"1\" locale=\"ja-jp\">" + angouka(sstg) + "</chat>");
            }));*/
        }

        private void haisutesyori(int n)
        {
            oto_binta.Play();

            ponken[mawariban] = false;
            hoflag[mawariban] = false;
            sutehai[mawariban][sutemax[mawariban]] = tehai[mawariban][n];
            sutemax[mawariban]++;
            tehai[mawariban][n] = tehai[mawariban][8];
            tehai[mawariban][8] = 100;
            Array.Sort(tehai[mawariban]);

            for (int ii = 0; ii < 4; ii++)
            {
                agarihantei(ii);
            }

            if (agariflag[0] || agariflag[1] || agariflag[2] || agariflag[3])
            {
                oto_ron.Play();
                Thread th = new Thread(tokuhatu);
                th.IsBackground = true;
                th.Start();                       
            }
            else
            {
                ponhantei();

                if (ponflag[0] || ponflag[1] || ponflag[2] || ponflag[3])
                {
                    if (jiseki > -1 && ponflag[jiseki])
                    {
                        oto_ponsuruno.Play();
                    }
                    else
                    {
                        ponuketuke();
                    }
                    /*
                    if (ponflag[0])
                    {
                        if (jiseki != 0)
                        {
                            dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[0] + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("ポン,0") + "</chat>");
                        }
                    }
                    if (ponflag[1])
                    {
                        if (jiseki != 1)
                        {
                            dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[1] + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("ポン,1") + "</chat>");
                        }
                    }
                    if (ponflag[2])
                    {
                        if (jiseki != 2)
                        {
                            dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[2] + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("ポン,2") + "</chat>");
                        }
                    }
                    if (ponflag[3])
                    {
                        if (jiseki != 3)
                        {
                            dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[3] + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("ポン,3") + "</chat>");
                        }
                    }
                    */
                }
                else
                {
                    genzaipaime++;
                    if (genzaipaime != 82)
                    {
                        mawariban++;
                        if (mawariban == 4)
                        {
                            mawariban = 0;
                        }

                        tehai[mawariban][8] = haijun[genzaipaime];
                        agarihantei(mawariban);
                        if (agariflag[mawariban])
                        {
                            oto_tumo.Play();
                            Thread th = new Thread(tokuhatu);
                            th.IsBackground = true;
                            th.Start();                           
                        }
                        else
                        {
                            if (mawariban == jiseki)
                            {
                                oto_torihai.Play();
                            }

                            if (reachflag[mawariban])
                            {
                                haisutesyori(8);
                            }
                            else
                            {
                                //人工無能
                                if (h_sankaid[mawariban] == "-1")
                                {
                                    haisutesyori(8);
                                }

                                if (mawariban != jiseki)
                                {
                                    //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[mawariban].ToString() + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("捨牌,0," + mawariban.ToString() + "," + Environment.TickCount.ToString()) + "</chat>");
                                }
                            }
                        }
                    }
                    else 
                    {
                        if (jibunid == m_userid)
                        {
                            Thread th = new Thread(kyokureset);
                            th.IsBackground = true;
                            th.Start();
                        }
                    }
                }
            }
        }

        private void kyokureset()
        {
            Thread.Sleep(3000);
            haipai();
        }
    }
}