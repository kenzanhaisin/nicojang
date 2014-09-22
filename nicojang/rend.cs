using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nicojang
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private int haiyoko = 38;
        private int haitate = 52;
        private int haioku = 32;
        private int norisiro = 4;
        private int namaekannkaku = 30;

        private void timer1_Tick(object sender, EventArgs e)
        {
             mt.WaitOne();

             using (Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height))
             {
                 g = Graphics.FromImage(bmp);

                 g.Clear(Color.White);
                 g.PixelOffsetMode = PixelOffsetMode.Half;

                 switch (stage)
                 {
                     case "初期":
                         g.DrawImage(Properties.Resources.nicojanglogo600600, 0, 0, pictureBox1.Width, pictureBox1.Height);

                         if (version == FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion)
                         {
                             g.DrawString(
                             "右上でブラウザを選択し放送ページのURLを入力してEnter\r\n複数のアカウントでログインしていると正常に動作しない場合がございます", new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.Red, syokie, sfnaka);
                         }
                         else
                         {
                             g.DrawString("バージョンアップしています。\r\n一度終了し最新バージョンにして下さい。", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Red, syokie, sfnaka);
                         }

                         break;

                     case "設定":
                         if (jibunid == m_userid || setuser)
                         {
                             if (jibunid != m_userid && setuser)
                             {
                                 g.DrawString("設定が決定しました\r\n雀士が決まるまでお待ち下さい", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Red, new Rectangle(0, 0, 600, 50), sfnaka);
                             }
                             else
                             {
                                 if (textBoxcharzoku.Visible)
                                 {
                                     if (sentakuchar > -1)
                                     {
                                         if (syutokusippai)
                                         {
                                             g.DrawString("プロフィールURLを入力しEnter", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Red, new Rectangle(0, 0, 600, 50), sfnaka);
                                         }
                                         else
                                         {
                                             g.DrawString("プロフィールを正常に取得できません", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Red, new Rectangle(0, 0, 600, 50), sfnaka);
                                         }

                                     }
                                     else if (sentakuzoku > -1)
                                     {
                                         g.DrawString("タグを入力しEnter(6文字まで)", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Red, new Rectangle(0, 0, 600, 50), sfnaka);
                                     }
                                 }
                                 else
                                 {
                                     g.DrawString("変更したい所をクリック", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Red, new Rectangle(0, 0, 600, 50), sfnaka);
                                 }
                             }
                             g.DrawLine(new Pen(Color.Black, 2), 150, 50, 550, 50);
                             g.DrawLine(new Pen(Color.Black, 2), 50, 150, 50, 550);
                             //縦線
                             for (int i = 0; i <= 20; i++)
                             {
                                 g.DrawLine(new Pen(Color.Black, 2), i * 20 + 150, 50, i * 20 + 150, 550);
                             }
                             //横線
                             for (int i = 0; i <= 10; i++)
                             {
                                 g.DrawLine(new Pen(Color.Black, 2), 50, i * 40 + 150, 550, i * 40 + 150);
                             }
                             //タグ名
                             for (int i = 0; i < 20; i++)
                             {
                                 if (sentakuzoku == i)
                                 {
                                     g.FillRectangle(Brushes.Black, new Rectangle(i * 20 + 151, 51, 18, 98));
                                     g.DrawString(p_zokusei[i], new Font("ＭＳ Ｐゴシック", 14, GraphicsUnit.Pixel), Brushes.White, new Rectangle(i * 20 + 151, 51, 18, 98), sftatesita);
                                 }
                                 else
                                 {
                                     g.DrawString(p_zokusei[i], new Font("ＭＳ Ｐゴシック", 14, GraphicsUnit.Pixel), Brushes.Black, new Rectangle(i * 20 + 151, 51, 18, 98), sftatesita);
                                 }
                             }
                             //キャラ
                             for (int j = 0; j < 10; j++)
                             {
                                 if (sentakuchar == j)
                                 {
                                     g.FillRectangle(Brushes.Black, new Rectangle(51, j * 40 + 151, 98, 38));
                                 }
                                 else
                                 {
                                     g.FillRectangle(namanusicolor[j], new Rectangle(51, j * 40 + 151, 98, 38));
                                 }

                                 g.DrawImage(p_charagazou28[j], 52, j * 40 + 151 + 5, 28, 28);
                                 g.DrawString(p_charaid[j], new Font("ＭＳ Ｐゴシック", 10, GraphicsUnit.Pixel), Brushes.White, new Rectangle(51 + 30, j * 40 + 151, 68, 19), sfnakahidari);
                                 g.DrawString(p_charaimei[j], new Font("ＭＳ Ｐゴシック", 10, GraphicsUnit.Pixel), Brushes.White, new Rectangle(51 + 30, j * 40 + 151 + 19, 68, 19), sfnakahidari);
                             }
                             //タグ値
                             for (int i = 0; i < 20; i++)
                             {
                                 for (int j = 0; j < 9; j++)
                                 {
                                     if (p_setteihyo[i, j])
                                     {
                                         g.FillRectangle(Brushes.Green, new Rectangle(i * 20 + 151, j * 40 + 151, 18, 38));
                                     }
                                 }
                             }

                             g.FillRectangle(Brushes.Black, new Rectangle(151, 511, 398, 38));
                             g.DrawString("オールマイティ", new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(151, 511, 398, 38), sfnaka);

                             g.FillRectangle(Brushes.Green, zokuyomi);
                             g.DrawString("設定読出", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, zokuyomi, sfnaka);

                             g.FillRectangle(Brushes.Green, zokukaki);
                             g.DrawString("設定保存", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, zokukaki, sfnaka);

                             g.FillRectangle(Brushes.Green, zokukaku);
                             g.DrawString("確定", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, zokukaku, sfnaka);
                         }
                         else
                         {
                             g.DrawString("主が設定中です。しばらくお待ち下さい。", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Red, new Rectangle(0, 0, 600, 50), sfnaka);
                             g.DrawString("・始めにサイコロを２コ振って親決め\r\n"+
                                          "・右の席から反時計回りに２・３・４・・・１２と数えて該当の人が親\r\n"+
                                          "・親は得点が１．５倍になる\r\n"+
                                          "・順番は親から初めて反時計回り\r\n"+
                                          "・最初の持ち点は２５０００点\r\n" +
                                          "・牌は９コ×９色にオールマイティ１個を加えて全部で８２コ\r\n"+
                                          "・基本は同色３コ×３組揃えれば上がれる（対々和１０００点）\r\n" +
                                          "・同色の組があっても上がれる（全部同じ色が揃えば一気通貫３００００点）\r\n"+
                                          "・上がるために数字は関係無い\r\n"+
                                          "・９コ違う色を揃えても上がれる（国士無双５０００点）\r\n" +
                                          "・同色牌を２コ以上持っていて他の席の人が同色牌を捨てたらポンできる\r\n"+
                                          "・ただしポンするととメンゼン１００００点は付かずリーチもできなくなる\r\n"+
                                          "・ロンで上がってもメンゼンは付かない\r\n" +
                                          "・ポンしたい人が複数いたら捨てた席の右隣りの人から優先される\r\n"+
                                          "・誰かが上がった場合、その人が親でなければ親は右隣りの人に移る\r\n" +
                                          "・親が一周したら対局終了"
                             , new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.Black, new Rectangle(50, 50, 500, 550));
                         }
                         break;

                     case "面子":
                             g.Clear(Color.Green);

                             if (jibunid != m_userid)
                             {
                                 g.DrawString("雀士が決定しました", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(0, 0, 600, 50), sfnaka);
                             }
                             else
                             {
                                 g.DrawString("右側で雀士を選択して枠をクリック", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(0, 0, 600, 50), sfnaka);
                                 //確定
                                 g.FillRectangle(Brushes.White, new Rectangle(250, 250, 100, 100));
                                 g.DrawString("確定", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.Green, new Rectangle(250, 250, 100, 100), sfnaka);
                             }
                                                          
                             //右
                             g.DrawImage(h_sankagazou[0], 400, 250, 100, 100);
                             g.DrawString(h_sankamei[0], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(300, 350, 300, 20), sfnaka);
                             //上
                             g.DrawImage(h_sankagazou[1], 250, 100, 100, 100);
                             g.DrawString(h_sankamei[1], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 200, 300, 20), sfnaka);
                             //左
                             g.DrawImage(h_sankagazou[2], 100, 250, 100, 100);
                             g.DrawString(h_sankamei[2], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(0, 350, 300, 20), sfnaka);
                             //下
                             g.DrawImage(h_sankagazou[3], 250, 400, 100, 100);
                             g.DrawString(h_sankamei[3], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 500, 300, 20), sfnaka);
                         
                         break;

                     case "親決":
                         g.Clear(Color.Green);
                         if (saiketu)
                         {
                             g.DrawString("初めの親は" + h_sankamei[oya] + "さんに決定しました", new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(0, 0, 600, 50), sfnaka);

                             g.DrawImage(sai[sai1], new Rectangle(255, 285, 30, 30));
                             g.DrawImage(sai[sai2], new Rectangle(315, 285, 30, 30));
                         }
                         else
                         {                            
                             g.DrawImage(sai[new Random(Environment.TickCount).Next(6)], new Rectangle(255, 285, 30, 30));
                             g.DrawImage(sai[new Random(new Random(Environment.TickCount).Next(100)).Next(6)], new Rectangle(315, 285, 30, 30));                            
                         }
                         break;

                     case"対局":
                         g.Clear(Color.Green);

                         //手牌
                         for (int i = 0; i < 8; i++)
                         {
                             //右
                             if (tehai[migi][i] != 100)
                             {
                                 if (agariflag[migi] || genzaipaime == 82)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(tehai[migi][i])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(pictureBox1.Width - haitate, pictureBox1.Height - (i * haiyoko + (haitate * 2) + haiyoko), haitate, haiyoko));
                                     }
                                 }
                                 else
                                 {
                                     using (Bitmap _bmp = new Bitmap(Properties.Resources.haiue3832))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(pictureBox1.Width - haioku, pictureBox1.Height - (i * haiyoko + (haitate * 2) + haiyoko), haioku, haiyoko));
                                     }
                                 }
                             }

                             //上
                             if (tehai[ue][i] != 100)
                             {
                                 if (agariflag[ue] || genzaipaime == 82)
                                 {
                                     using (Bitmap _bmp = new Bitmap( haigaesi(tehai[ue][i])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(pictureBox1.Width - (i * haiyoko + (haitate * 2) + haiyoko), 0, haiyoko, haitate));
                                     }
                                 }
                                 else
                                 {
                                     g.DrawImage(Properties.Resources.haiue3832, new Rectangle(pictureBox1.Width - (i * haiyoko + (haitate * 2) + haiyoko), 0, haiyoko, haioku));
                                 }
                             }

                             //左
                             if (tehai[hidari][i] != 100)
                             {
                                 if (agariflag[hidari] || genzaipaime == 82)
                                 {
                                     using (Bitmap _bmp =  new Bitmap(haigaesi(tehai[hidari][i])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(0, i * haiyoko + (haitate * 2), haitate, haiyoko));
                                     }
                                 }
                                 else
                                 {
                                     using (Bitmap _bmp =  new Bitmap(Properties.Resources.haiue3832))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(0, i * haiyoko + (haitate * 2), haioku, haiyoko));
                                     }
                                 }
                             }

                             //下
                             if (tehai[sita][i] != 100)
                             {
                                 g.DrawImage(haigaesi(tehai[sita][i]), new Rectangle(i * haiyoko + haitate * 2, pictureBox1.Height - haitate, haiyoko, haitate));
                             }
                         }
                                                                           
                         //配牌
                         if (migi == mawariban)
                         {//右
                             if (tehai[migi][8] != 100)
                             {
                                 if (agariflag[migi] || genzaipaime == 82)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(tehai[migi][8])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(pictureBox1.Width - haitate, 180 - haiyoko, haitate, haiyoko));
                                     }
                                 }
                                 else
                                 {
                                     using (Bitmap _bmp = new Bitmap(Properties.Resources.haiue3832))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(pictureBox1.Width - haioku, 180 - haiyoko, haioku, haiyoko));
                                     }
                                 }
                             }
                         }
                         else if (ue == mawariban)
                         {//上
                             if (tehai[ue][8] != 100)
                             {
                                 if (agariflag[ue] || genzaipaime == 82)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(tehai[ue][8])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(142, 0, haiyoko, haitate));
                                     }
                                 }
                                 else
                                 {
                                     g.DrawImage(Properties.Resources.haiue3832, new Rectangle(142, 0, haiyoko, haioku));
                                 }
                             }
                         }
                         else if (hidari == mawariban)
                         {//左
                             if (tehai[hidari][8] != 100)
                             {
                                 if (agariflag[hidari] || genzaipaime == 82)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(tehai[hidari][8])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(0, 420, haitate, haiyoko));
                                     }
                                 }
                                 else
                                 {
                                     using (Bitmap _bmp = new Bitmap(Properties.Resources.haiue3832))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(0, 420, haioku, haiyoko));
                                     }
                                 }
                             }
                         }
                         else if (sita == mawariban)
                         {//下
                             if (tehai[sita][8] != 100)
                             {
                                 g.DrawImage(haigaesi(tehai[nagame][8]), new Rectangle(420, pictureBox1.Height - haitate, haiyoko, haitate));
                             }
                         }
                         
                         //捨牌
                         for (int j = 0; j < 2; j++)
                         {
                             for (int i = 0; j * 7 + i < sutemax[migi] && i < 7; i++)
                             {//右
                                 if (sutehai[migi][(j * 7) + i] != 100)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(sutehai[migi][(j * 7) + i])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(433 + (haitate * j), 433 - (haiyoko * i) - haiyoko, haitate, haiyoko));
                                     }
                                 }
                             }
                         }
                         for (int j = 0; j < 2; j++)
                         {
                             for (int i = 0; j * 7 + i < sutemax[ue] && i < 7; i++)
                             {//上
                                 if (sutehai[ue][(j * 7) + i] != 100)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(sutehai[ue][(j * 7) + i])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(433 - haiyoko - (i * haiyoko), 167 - haitate - (haitate * j), haiyoko, haitate));
                                     }
                                 }
                             }
                         }
                         for (int j = 0; j < 2; j++)
                         {
                             for (int i = 0; j * 7 + i < sutemax[hidari] && i < 7; i++)
                             {//左
                                 if (sutehai[hidari][(j * 7) + i] != 100)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(sutehai[hidari][(j * 7) + i])))
                                     {
                                         _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                         g.DrawImage(_bmp, new Rectangle(167 - haitate - (haitate * j), i * haiyoko + 167, haitate, haiyoko));
                                     }
                                 }
                             }
                         }
                         for (int j = 0; j < 2; j++)
                         {
                             for (int i = 0; j * 7 + i < sutemax[sita] && i < 7; i++)
                             {//下
                                 if (sutehai[sita][(j * 7) + i] != 100)
                                 {
                                     g.DrawImage(haigaesi(sutehai[sita][(j * 7) + i]), new Rectangle((haiyoko * i) + 167, 433 + (haitate * j), haiyoko, haitate));
                                 }
                             }
                         }

                         //ポン枠
                         if (ponflag[0] || ponflag[1] || ponflag[2] || ponflag[3])
                         {
                             if (mawariban == migi)
                             {
                                 if (sutemax[migi] <= 7)
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle(433, 433 - (haiyoko * (sutemax[migi] - 1)) - haiyoko, haitate, haiyoko));
                                 }
                                 else
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle(433 + haitate, 433 - (haiyoko * (sutemax[migi] - 1 - 7)) - haiyoko, haitate, haiyoko));
                                 }                                 
                             }
                             else if(mawariban == ue)
                             {
                                 if (sutemax[ue] <= 7)
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle(433 - haiyoko - ((sutemax[ue] - 1) * haiyoko), 167 - haitate , haiyoko, haitate));
                                 }
                                 else
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle(433 - haiyoko - ((sutemax[ue] - 1 - 7) * haiyoko), 167 - haitate - haitate, haiyoko, haitate));
                                 } 
                             }
                             else if(mawariban == hidari)
                             {
                                 if (sutemax[hidari] <= 7)
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle(167 - haitate , (sutemax[hidari] - 1) * haiyoko + 167, haitate, haiyoko));
                                 }
                                 else
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle(167 - haitate - haitate, (sutemax[hidari] - 1 - 7) * haiyoko + 167, haitate, haiyoko));
                                 } 
                             }
                             else if (mawariban == sita)
                             {
                                 if (sutemax[sita] <= 7)
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle((haiyoko * (sutemax[sita] - 1)) + 167, 433 , haiyoko, haitate));
                                 }
                                 else
                                 {
                                     g.DrawRectangle(new Pen(Brushes.Blue, 3), new Rectangle((haiyoko * (sutemax[sita] - 1 - 7)) + 167, 433 + haitate, haiyoko, haitate));
                                 } 
                             }
                         }

                         //ポン
                         for (int j = 0; j < 2; j++)
                         {//右
                             if (pondarekara[migi][j] > -1)
                             {
                                 int naga = 128;
                                 for (int k = 0; k < 3; k++)
                                 {
                                     using (Bitmap _bmp = new Bitmap(haigaesi(pon[migi][j, k])))
                                     {
                                         if (migi - (k + 1) == pondarekara[migi][j] || migi - (k + 1) + 4 == pondarekara[migi][j])
                                         {
                                             _bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                             naga = naga - haitate;
                                             g.DrawImage(_bmp, new Rectangle(pictureBox1.Width - ((j + 1) * haitate) + haitate - haiyoko, naga, haiyoko, haitate));
                                         }
                                         else
                                         {
                                             _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                             naga = naga - haiyoko;
                                             g.DrawImage(_bmp, new Rectangle(pictureBox1.Width - (((j + 1) * haitate)), naga, haitate, haiyoko));
                                         }
                                     }
                                 }
                             }
                         }
                         for (int j = 0; j < 2; j++)
                         {//上
                             if (pondarekara[ue][j] > -1)
                             {
                                 int naga = 128;
                                 for (int k = 0; k < 3; k++)
                                 {
                                     using (Bitmap _bmp =  new Bitmap(haigaesi(pon[ue][j, k])))
                                     {
                                         if (ue - (k + 1) == pondarekara[ue][j] || ue - (k + 1) + 4 == pondarekara[ue][j])
                                         {
                                             _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                             naga = naga - haitate;
                                             g.DrawImage(_bmp, new Rectangle(naga, j * haitate, haitate, haiyoko));
                                         }
                                         else
                                         {
                                             _bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                             naga = naga - haiyoko;
                                             g.DrawImage(_bmp, new Rectangle(naga, j * haitate, haiyoko, haitate));
                                         }
                                     }
                                 }
                             }
                         }
                         for (int j = 0; j < 2; j++)
                         {//左
                             if (pondarekara[hidari][j] > -1)
                             {
                                 int naga = pictureBox1.Height - 128;
                                 for (int k = 0; k < 3; k++)
                                 {
                                     using (Bitmap _bmp =  new Bitmap(haigaesi(pon[hidari][j, k])))
                                     {
                                         if (hidari - (k + 1) == pondarekara[hidari][j] || hidari - (k + 1) + 4 == pondarekara[hidari][j])
                                         {                                            
                                             g.DrawImage(_bmp, new Rectangle(j * haitate, naga, haiyoko, haitate));
                                             naga = naga + haitate;
                                         }
                                         else
                                         {
                                             _bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);                                             
                                             g.DrawImage(_bmp, new Rectangle( j * haitate, naga, haitate, haiyoko));
                                             naga = naga + haiyoko;
                                         }
                                     }
                                 }
                             }
                         }
                         for (int j = 0; j < 2; j++)
                         {//下
                             if (pondarekara[sita][j] > -1)
                             {
                                 int naga = pictureBox1.Width - 128;
                                 for (int k = 0; k < 3; k++)
                                 {
                                     using (Bitmap _bmp =  new Bitmap(haigaesi(pon[sita][j, k])))
                                     {
                                         if (sita - (k + 1) == pondarekara[sita][j] || sita - (k + 1) + 4 == pondarekara[sita][j])
                                         {
                                             _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                             g.DrawImage(_bmp, new Rectangle(naga, pictureBox1.Height - ((j + 1) * haitate) + (haitate - haiyoko), haitate, haiyoko));
                                             naga = naga + haitate;
                                         }
                                         else
                                         {                                             
                                             g.DrawImage(_bmp, new Rectangle(naga, pictureBox1.Height - ((j + 1) * haitate), haiyoko, haitate));
                                             naga = naga + haiyoko;
                                         }
                                     }
                                 }
                             }
                         }

                         //名前 
                         if (ponken[migi])
                         {//右
                             g.FillRectangle(Brushes.Blue, miginameerea);
                             g.DrawString("ポン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, miginameerea, sftate);
                         }
                         else if (agariflag[migi])
                         {
                             g.FillRectangle(Brushes.Red, miginameerea);
                             if (tehai[migi][8] != 100)
                             {
                                 g.DrawString("ツモ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, miginameerea, sftate);
                             }
                             else
                             {
                                 g.DrawString("ロン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, miginameerea, sftate);
                             }
                         }
                         else if (reachflag[migi])
                         {
                             g.FillRectangle(Brushes.Pink, miginameerea);
                             g.DrawString("リーチ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.Black, miginameerea, sftate);
                         }    
                         else
                         {
                             g.DrawString(h_sankamei[migi], new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, miginameerea, sftate);
                         }

                         if (ponken[ue])
                         {//上
                             g.FillRectangle(Brushes.Blue, uenamearea);
                             g.DrawString("ポン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, uenamearea, sfnaka);
                         }
                         else if (agariflag[ue])
                         {
                             g.FillRectangle(Brushes.Red, uenamearea);
                             if (tehai[ue][8] != 100)
                             {
                                 g.DrawString("ツモ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, uenamearea, sfnaka);
                             }
                             else
                             {
                                 g.DrawString("ロン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, uenamearea, sfnaka);
                             }
                         }
                         else if (reachflag[ue])
                         {
                             g.FillRectangle(Brushes.Pink, uenamearea);
                             g.DrawString("リーチ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.Black, uenamearea, sfnaka);
                         }
                         else
                         {
                             g.DrawString(h_sankamei[ue], new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, uenamearea, sfnaka);
                         }

                         if (ponken[hidari])
                         {//左
                             g.FillRectangle(Brushes.Blue, hidarinamearea);
                             g.DrawString("ポン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, hidarinamearea, sftate);
                         }
                         else if (agariflag[hidari])
                         {
                             g.FillRectangle(Brushes.Red, hidarinamearea);
                             if (tehai[hidari][8] != 100)
                             {
                                 g.DrawString("ツモ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, hidarinamearea, sftate);
                             }
                             else
                             {
                                 g.DrawString("ロン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, hidarinamearea, sftate);
                             }
                         }
                         else if (reachflag[hidari])
                         {
                             g.FillRectangle(Brushes.Pink, hidarinamearea);
                             g.DrawString("リーチ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.Black, hidarinamearea, sftate);
                         }  
                         else
                         {
                             g.DrawString(h_sankamei[hidari], new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, hidarinamearea, sftate);
                         }

                         if (ponken[sita])
                         {//下
                             g.FillRectangle(Brushes.Blue, sitanameerea);
                             g.DrawString("ポン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, sitanameerea, sfnaka);
                         }
                         else if (agariflag[sita])
                         {
                             g.FillRectangle(Brushes.Red, sitanameerea);
                             if (tehai[sita][8] != 100)
                             {
                                 g.DrawString("ツモ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, sitanameerea, sfnaka);
                             }
                             else
                             {
                                 g.DrawString("ロン", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, sitanameerea, sfnaka);
                             }
                         }
                         else if (reachflag[sita])
                         {
                             g.FillRectangle(Brushes.Pink, sitanameerea);
                             g.DrawString("リーチ", new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.Black, sitanameerea, sfnaka);
                         }  
                         else
                         {
                             g.DrawString(h_sankamei[sita], new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, sitanameerea, sfnaka);
                         }

                         //親プレート
                         if (migi == mawariban)
                         {//右
                             g.FillRectangle(Brushes.Black, new Rectangle(373, 197, 30, 206));
                         }
                         else if (ue == mawariban)
                         {//上
                             g.FillRectangle(Brushes.Black, new Rectangle(197, 197, 206, 30));
                         }
                         else if (hidari == mawariban)
                         {//左
                             g.FillRectangle(Brushes.Black, new Rectangle(197, 197, 30, 206));
                         }
                         else if (sita == mawariban)
                         {//下
                             g.FillRectangle(Brushes.Black, new Rectangle(197, 373, 206, 30));
                         }

                         //親
                         if (migi == oya)
                         {//右
                             g.DrawImage(Properties.Resources.oya, new Rectangle(373,285, 30, 30));
                         }
                         else if (ue == oya)
                         {//上
                             g.DrawImage(Properties.Resources.oya, new Rectangle(285,197, 30, 30));
                         }
                         else if (hidari == oya)
                         {//左
                             g.DrawImage(Properties.Resources.oya, new Rectangle(197,285, 30, 30));
                         }
                         else if (sita == oya)
                         {//下
                             g.DrawImage(Properties.Resources.oya, new Rectangle(285,373, 30, 30));
                         }
                         
                         //コメント上
                         if (genzaipaime == 82)
                         {
                             g.DrawString("流局", new Font("ＭＳ 明朝", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(227, 227, 146, 46), sfnaka);
                         }
                         else
                         {
                             g.DrawString("残り" + (82 - genzaipaime).ToString(), new Font("ＭＳ 明朝", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(227, 227, 146, 46), sfnaka);
                         }
                         //コメント下
                         if (jiseki > -1)
                         {
                             if (ponflag[0] || ponflag[1] || ponflag[2] || ponflag[3])
                             {
                                 g.DrawString("ポン判定中", new Font("ＭＳ 明朝", 12, GraphicsUnit.Pixel), Brushes.White, new Rectangle(227, 273, 146, 100), sfnaka);
                             }
                             else
                             {
                                 g.DrawString("同色３個×３組か\r\n全部違う色を\r\n揃えよう", new Font("ＭＳ 明朝", 12, GraphicsUnit.Pixel), Brushes.White, new Rectangle(227, 273, 146, 100), sfnaka);
                             }
                         }
                         else
                         {
                             g.DrawString("名前をクリックすると\r\nその雀士の目線で\r\n見られます", new Font("ＭＳ 明朝", 12, GraphicsUnit.Pixel), Brushes.White, new Rectangle(227, 273, 146, 100), sfnaka);
                         }

                         //マーク
                         if (jiseki > -1 && ponflag[sita])
                         {
                             g.FillRectangle(Brushes.Pink, nonerea);
                             g.FillRectangle(Brushes.Pink, ponreacherea);
                             g.DrawString("パ　ス", new Font("ＭＳ 明朝", 10, GraphicsUnit.Pixel), Brushes.Black, nonerea, sfnaka);
                             g.DrawString("ポ　ン", new Font("ＭＳ 明朝", 10, GraphicsUnit.Pixel), Brushes.Black, ponreacherea, sfnaka);
                         }
                         break;

                     case"得発":
                         g.Clear(Color.Green);

                         g.DrawImage(h_sankagazou[genaka], 50, 50, 100, 100);
                         g.DrawString(h_sankamei[genaka], new Font("ＭＳ 明朝", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(50, 150, 100, 20), sfnaka);

                         if (tokuflag > 0)
                         {
                             for (int x = 0; x < tokuflag; x++)
                             {
                                 g.DrawString(y_yaku[x][0, 0], new Font("ＭＳ 明朝", 18, GraphicsUnit.Pixel), Brushes.White, new Rectangle(200, x * 20 + 50, 200, 20), sfnakahidari);
                                 g.DrawString(y_yaku[x][1, 0], new Font("ＭＳ 明朝", 18, GraphicsUnit.Pixel), Brushes.White, new Rectangle(400, x * 20 + 50, 200, 20), sfnakamigi);
                             }
                         }

                         if (sougoutokuten)
                         {
                             g.DrawLine(new Pen(Brushes.White, 1), 350, y_yaku.Count * 20 + 50, 600, y_yaku.Count * 20 + 50);
                             g.DrawString((henten[genaka] * 100).ToString(), new Font("ＭＳ 明朝", 18, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, new Rectangle(400, y_yaku.Count * 20 + 50, 200, 20), sfnakamigi);
                         }

                         int[] junpai = new int[9];
                         tehai[genaka].CopyTo(junpai, 0);
                         if (pondarekara[genaka][0] > -1 && pondarekara[genaka][1] > -1)
                         {
                             junpai[2] = pon[genaka][0, 0];
                             junpai[3] = pon[genaka][0, 1];
                             junpai[4] = pon[genaka][0, 2];
                             junpai[5] = pon[genaka][1, 0];
                             junpai[6] = pon[genaka][1, 1];
                             junpai[7] = pon[genaka][1, 2];
                         }
                         else if (pondarekara[genaka][0] > -1)
                         {
                             junpai[5] = pon[genaka][0, 0];
                             junpai[6] = pon[genaka][0, 1];
                             junpai[7] = pon[genaka][0, 2];
                         }

                         if (junpai[8] == 100)
                         {
                             junpai[8] = sutehai[mawariban][sutemax[mawariban] - 1];
                         }

                         for (int i = 0; i < 3; i++)
                         {
                             for (int j = 0; j < 3; j++)
                             {
                                 g.DrawImage(haigaesi(junpai[i * 3 + j]), new Rectangle(j * haiyoko + 43, i * haitate + 200, haiyoko, haitate));
                             }
                         }
                             break;

                     case"中得":
                         g.Clear(Color.Green);
                      
                         //右
                         if (henten[migi] > 0)
                         {
                             g.DrawString("+" + (henten[migi] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(400, 210, 100, 20), sfnakamigi);
                         }
                         else if (henten[migi] < 0)
                         {
                             g.DrawString((henten[migi] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(400, 210, 100, 20), sfnakamigi);
                         }
                         g.DrawString((tokuten[migi]* 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(400, 230, 100, 20), sfnakamigi);
                         g.DrawImage(h_sankagazou[migi], 400, 250, 100, 100);
                         g.DrawString(h_sankamei[migi], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(300, 350, 300, 20), sfnaka);
                         //上
                         if (henten[ue] > 0)
                         {
                             g.DrawString("+" + (henten[ue] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(250, 60, 100, 20), sfnakamigi);
                         }
                         else if (henten[ue] < 0)
                         {
                             g.DrawString((henten[ue] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(250, 60, 100, 20), sfnakamigi);
                         }
                         g.DrawString((tokuten[ue]* 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(250, 80, 100, 20), sfnakamigi);
                         g.DrawImage(h_sankagazou[ue], 250, 100, 100, 100);
                         g.DrawString(h_sankamei[ue], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 200, 300, 20), sfnaka);
                         //左
                         if (henten[hidari] > 0)
                         {
                             g.DrawString("+" + (henten[hidari] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(100, 210, 100, 20), sfnakamigi);
                         }
                         else if (henten[hidari] < 0)
                         {
                             g.DrawString((henten[hidari] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(100, 210, 100, 20), sfnakamigi);
                         }
                         g.DrawString((tokuten[hidari]* 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(100, 230, 100, 20), sfnakamigi);
                         g.DrawImage(h_sankagazou[hidari], 100, 250, 100, 100);
                         g.DrawString(h_sankamei[hidari], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(0, 350, 300, 20), sfnaka);
                         //下
                         if (henten[sita] > 0)
                         {
                             g.DrawString("+" + (henten[sita] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(250, 360, 100, 20), sfnakamigi);
                         }
                         else if (henten[sita] < 0)
                         {
                             g.DrawString((henten[sita] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(250, 360, 100, 20), sfnakamigi);
                         }
                         g.DrawString((tokuten[sita]* 100).ToString(), new Font("ＭＳ Ｐゴシック", 15, GraphicsUnit.Pixel), Brushes.White, new Rectangle(250, 380, 100, 20), sfnakamigi);
                         g.DrawImage(h_sankagazou[sita], 250, 400, 100, 100);
                         g.DrawString(h_sankamei[sita], new Font("ＭＳ Ｐゴシック", 20, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 500, 300, 20), sfnaka);    
                         break;

                     case"結果":
                         g.Clear(Color.Green);
                         //画像
                         g.DrawImage(h_sankagazou[narabikaetoku[3]], 40, 40, 100, 100);
                         g.DrawImage(h_sankagazou[narabikaetoku[2]], 40, 180, 100, 100);
                         g.DrawImage(h_sankagazou[narabikaetoku[1]], 40, 320, 100, 100);
                         g.DrawImage(h_sankagazou[narabikaetoku[0]], 40, 460, 100, 100);
                         //得点
                         g.DrawString((tokuten[narabikaetoku[3]] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 40, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 40, 450, 50), sfnakahidari);
                         g.DrawString((tokuten[narabikaetoku[2]] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 35, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 180, 450, 50), sfnakahidari);
                         g.DrawString((tokuten[narabikaetoku[1]] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 30, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 320, 450, 50), sfnakahidari);
                         g.DrawString((tokuten[narabikaetoku[0]] * 100).ToString(), new Font("ＭＳ Ｐゴシック", 25, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 460, 450, 50), sfnakahidari);
                         //名前
                         g.DrawString(h_sankamei[narabikaetoku[3]], new Font("ＭＳ Ｐゴシック", 40, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 90, 450, 50), sfnakahidari);
                         g.DrawString(h_sankamei[narabikaetoku[2]], new Font("ＭＳ Ｐゴシック", 35, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 230, 450, 50), sfnakahidari);
                         g.DrawString(h_sankamei[narabikaetoku[1]], new Font("ＭＳ Ｐゴシック", 30, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 370, 450, 50), sfnakahidari);
                         g.DrawString(h_sankamei[narabikaetoku[0]], new Font("ＭＳ Ｐゴシック", 25, GraphicsUnit.Pixel), Brushes.White, new Rectangle(150, 510, 450, 50), sfnakahidari);
                         break;
                 }

                pictureBox1.CreateGraphics().DrawImage(bmp, 0, 0);

                g.Dispose();
            }

            mt.ReleaseMutex();
        }

        private Bitmap haigaesi(int a)
        {
            if (a == 90)
            {
                return hai_all;
            }
            else
            {
                return hai[a / 10, a % 10];
            }
        }
    }
}