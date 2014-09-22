using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml;

namespace nicojang
{
    public partial class Form1 : Form
    {
        private void agarihantei(int n)
        {
            int[] junpai = new int[9];
            tehai[n].CopyTo(junpai, 0);

            int sanko = 0;
            int niko = 0;
            int ikko = 1;
            int kiri = 9;

            if (n != mawariban && junpai[8] == 100)
            {
                junpai[8] = sutehai[mawariban][sutemax[mawariban] - 1];
            }

            if (pondarekara[n][0] > -1 && pondarekara[n][1] > -1)
            {               
                sanko = 2;
                kiri = 3;
            }
            else if (pondarekara[n][0] > -1)
            {
                sanko = 1;
                kiri = 6;
            }

            if (n == mawariban && tehai[n][8] == 100)
            {
                kiri--;
            }

            Array.Sort(junpai);

            //オールマイティ判定
            bool al = false;
            if (junpai[1] == 90 || junpai[4] == 90 || junpai[7] == 90 || junpai[8] == 90)
            {
                al = true;
            }
            int kijun = 0;
            //３個２個１個判定
            for (int mokuhyo = 1; mokuhyo < kiri; mokuhyo++)
            {
                if (junpai[mokuhyo] / 10 == junpai[kijun] / 10)
                {
                    if (mokuhyo - kijun == 2)
                    {
                        mokuhyo++;
                        kijun = mokuhyo;
                        sanko++;
                        niko--;
                    }
                    else if (mokuhyo - kijun == 1)
                    {
                        niko++;
                        ikko--;
                    }
                }
                else
                {                    
                    if (mokuhyo - kijun == 2)
                    {
                        if(al)
                        {
                            al = false;
                            sanko++;
                            niko--;
                        }
                    }

                    kijun = mokuhyo;
                    ikko++;
                }
            }

            if (sanko == 3 || ikko == 9)
            {//あがり
                agariflag[n] = true;
            }
            else if (!reachflag[n] && pondarekara[n][0] == -1 && tehai[n][8] == 100 && ((sanko == 2 && niko == 1 && n == mawariban) || (ikko == 8 && n == mawariban)))
            {//聴牌
                reachflag[n] = true;
                reaippatu[n] = true;
                if (doblereahantei[n] && sutemax[mawariban] == 1)
                {
                    doblerea[n] = true;
                }
                oto_reach.Play();
            }
            else if(reachflag[n] && reaippatu[n] && n == mawariban)
            {
                reaippatu[n] = false;
            }
        }
     
        private int genaka = 0;
        private int[] narabikaetoku = new int[4];
        private bool sougoutokuten = false;
        private void tokuhatu()
        {         
            Thread.Sleep(5000);
            genaka = 0;           
            
            for (genaka = 0; genaka < 4; genaka++)
            {
                if (agariflag[genaka])
                {
                    tokuflag = 0;
                    sougoutokuten = false;
                    yakudukuri(genaka);
                    stage = "得発";

                    do
                    {                        
                        Thread.Sleep(1000);
                        Invoke(new dldl(delegate
                        {
                            oto_naguri.Play();
                            tokuflag++;
                            
                        }));                                
                    }
                    while (tokuflag < y_yaku.Count);

                    Thread.Sleep(1000);
                    Invoke(new dldl(delegate
                    {
                        oto_naguri.Play();
                        sougoutokuten = true;
                    }));

                    Thread.Sleep(5000);
                    stage = "中得";
                    Thread.Sleep(5000);
                }
            }

            bool aga = false;
            if (!agariflag[oya])
            {
                aga = true;
                oya++;
                if (oya == 4)
                {
                    oya = 0;
                }
            }

            if (!(aga && oya == firstoya) || oya != firstoya)
            {
                if (jibunid == m_userid)
                {
                    kyokureset();
                }
            }
            else
            {
                narabikaetoku[0] = 0;
                narabikaetoku[1] = 1;
                narabikaetoku[2] = 2;
                narabikaetoku[3] = 3;
                double[] daitoku = new double[4];
                tokuten.CopyTo(daitoku, 0);
                Array.Sort(daitoku, narabikaetoku);
                stage = "結果";
                setuser = false;
                stflag = "##属設";
                Thread.Sleep(10000);                
                stage = "設定";
            }
        }

        private void yakudukuri(int n)
        {
            y_yaku.Clear();
            henten = new double[4] { 0, 0, 0, 0 };

            if (doblerea[n])
            {//Wリーチ
                y_yaku.Add(new string[,] { { "ダブルリーチ" }, { "2000" } });
                henten[n] += 20;
            }
            else if (reachflag[n])
            {//リーチ
                y_yaku.Add(new string[,] { { "リーチ" }, { "1000" } });
                henten[n] += 10;
            }

            if (reaippatu[n])
            {//一発
                y_yaku.Add(new string[,] { { "イッパツ" }, { "1000" } });
                henten[n] += 10;
            }

            if (tehai[n][8] != 100 && pondarekara[n][0] == -1)
            {//メンゼン
                y_yaku.Add(new string[,] { { "メンゼン" }, { "10000" } });
                henten[n] += 100;
            }

            if (genzaipaime == 81)
            {//ハイテイ
                y_yaku.Add(new string[,] { { "ハイテイ" }, { "1000" } });
                henten[n] += 10;
            }
            
            //国士判定準備            
            int[] junpai = new int[9];
            tehai[n].CopyTo(junpai, 0);
            if (pondarekara[n][0] > -1 && pondarekara[n][1] > -1)
            {
                junpai[2] = pon[n][0, 0];
                junpai[3] = pon[n][0, 1];
                junpai[4] = pon[n][0, 2];
                junpai[5] = pon[n][1, 0];
                junpai[6] = pon[n][1, 1];
                junpai[7] = pon[n][1, 2];
            }
            else if (pondarekara[n][0] > -1)
            {
                junpai[5] = pon[n][0, 0];
                junpai[6] = pon[n][0, 1];
                junpai[7] = pon[n][0, 2];
            }

            if (junpai[8] == 100)
            {
                junpai[8] = sutehai[mawariban][sutemax[mawariban] - 1];
            }

            Array.Sort(junpai);
            bool kokusi = true;
            bool junkokusi = true;
            bool tinkokusi = true;
            bool[] kokusiban = new bool[9] { true, true, true, true, true, true, true, true, true };

            if (pondarekara[n][0] > -1)
            {
                kokusi = false;
            }

            for (int i = 0; i < 8; i++)
            {//国士無双判定
                if (junpai[i] / 10 == junpai[i + 1] / 10)
                {
                    kokusi = false;
                    break;
                }
            }
           
            if (kokusi)
            {
                //純国士無双判定
                int ban = junpai[0] % 10;
                for (int i = 1; i < 9; i++)
                {
                    if (junpai[i] != 90)
                    {
                        if (ban != junpai[i] % 10)
                        {
                            junkokusi = false;
                            break;
                        }
                    }
                }

                //清国士無双判定
                for (int i = 0; i < 9; i++)
                {
                    if (junpai[i] != 90)
                    {
                        if (kokusiban[junpai[i] % 10])
                        {
                            kokusiban[junpai[i] % 10] = false;
                        }
                        else
                        {
                            tinkokusi = false;
                            break;
                        }
                    }
                }
            }

            if (kokusi && junkokusi)
            {//純国士無双                
                y_yaku.Add(new string[,] { { "純国士無双" }, { "20000" } });
                henten[n] += 200;
            }
            else if (kokusi && tinkokusi)
            {//清国士無双
                y_yaku.Add(new string[,] { { "清国士無双" }, { "10000" } });
                henten[n] += 100;
            }
            else if (kokusi)
            {//国士無双                
                y_yaku.Add(new string[,] { { "国士無双" }, { "5000" } });
                henten[n] += 50;
            }
            else
            {//対々和
                y_yaku.Add(new string[,] { { "対々和" }, { "1000" } });
                henten[n] += 10;

                //三色同番
                if (junpai[0] / 10 != junpai[3] / 10 && junpai[3] / 10 != junpai[6] / 10)
                {
                    if (junpai[8] == 90)
                    {
                        int wareme = 0;
                        int[] douban = new int[3];
                        if (junpai[0] / 10 == junpai[1] / 10 && junpai[1] / 10 == junpai[2] / 10)
                        {
                            if (junpai[3] / 10 == junpai[4] / 10 && junpai[4] / 10 == junpai[5] / 10)
                            {
                                wareme = 3;
                                douban = new int[3] { junpai[0] % 10, junpai[1] % 10, junpai[2] % 10 };
                            }
                            else
                            {
                                wareme = 2;
                                douban = new int[3] { junpai[5] % 10, junpai[6] % 10, junpai[7] % 10 };
                            }
                        }
                        else
                        {
                            wareme = 1;
                            douban = new int[3] { junpai[2] % 10, junpai[3] % 10, junpai[4] % 10 };
                        }

                        switch (wareme)
                        {
                            case 1:
                                if (douban[0] == junpai[5] % 10 && douban[1] == junpai[6] % 10 && douban[2] == junpai[7] % 10)
                                {
                                    if (Array.IndexOf(douban, junpai[0] % 10) > -1 && Array.IndexOf(douban, junpai[1] % 10) > -1)
                                    {
                                        y_yaku.Add(new string[,] { { "三色同番" }, { "30000" } });
                                        henten[n] += 300;
                                    }
                                }
                                break;
                            case 2:
                                if (douban[0] == junpai[0] % 10 && douban[1] == junpai[1] % 10 && douban[2] == junpai[2] % 10)
                                {
                                    if (Array.IndexOf(douban, junpai[3] % 10) > -1 && Array.IndexOf(douban, junpai[4] % 10) > -1)
                                    {
                                        y_yaku.Add(new string[,] { { "三色同番" }, { "30000" } });
                                        henten[n] += 300;
                                    }
                                }
                                break;
                            case 3:
                                if (douban[0] == junpai[3] % 10 && douban[1] == junpai[4] % 10 && douban[2] == junpai[5] % 10)
                                {
                                    if (Array.IndexOf(douban, junpai[6] % 10) > -1 && Array.IndexOf(douban, junpai[7] % 10) > -1)
                                    {
                                        y_yaku.Add(new string[,] { { "三色同番" }, { "30000" } });
                                        henten[n] += 300;
                                    }
                                }
                                break;
                        }

                    }
                    else
                    {
                        if (junpai[0] % 10 == junpai[3] % 10 && junpai[3] % 10 == junpai[6] % 10)
                        {
                            if (junpai[1] % 10 == junpai[4] % 10 && junpai[4] % 10 == junpai[7] % 10)
                            {
                                if (junpai[2] % 10 == junpai[5] % 10 && junpai[5] % 10 == junpai[8] % 10)
                                {
                                    y_yaku.Add(new string[,] { { "三色同番" }, { "30000" } });
                                    henten[n] += 300;
                                }
                            }
                        }
                    }
                }
            }

            if (tehai[n][0] / 10 == tehai[n][3] / 10 && tehai[n][3] / 10 == tehai[n][6] / 10)
            {//一気通貫
                y_yaku.Add(new string[,] { { "一気通貫" }, { "30000" } });
                henten[n] += 300;
            }

            if (hoflag[n] && mawariban == n && tehai[n][8] != 100)
            {//天和
                y_yaku.Add(new string[,] { { "天和" }, { "30000" } });
                henten[n] += 300;
            }
            else if (hoflag[n] && tehai[n][8] != 100)
            {//地和
                y_yaku.Add(new string[,] { { "地和" }, { "30000" } });
                henten[n] += 300;
            }
            else if (hoflag[n] && tehai[n][8] == 100)
            {//人和
                y_yaku.Add(new string[,] { { "人和" }, { "30000" } });
                henten[n] += 300;
            }

            if (!kokusi)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (p_zokusei[i] != "")
                    {
                        if (p_setteihyo[i, junpai[0] / 10] && p_setteihyo[i, junpai[3] / 10] && p_setteihyo[i, junpai[6] / 10])
                        {
                            if ((junpai[0] / 10 == junpai[3] / 10 && junpai[3] / 10 == junpai[6] / 10 && junpai[6] / 10 == junpai[0] / 10) || (junpai[0] / 10 != junpai[3] / 10 && junpai[3] / 10 != junpai[6] / 10 && junpai[6] / 10 != junpai[0] / 10))
                            {
                                y_yaku.Add(new string[,] { { "清 " + p_zokusei[i] }, { "3000" } });
                                henten[n] += 30;
                            }
                        }
                        else if ((junpai[0] / 10 != junpai[3] / 10 && p_setteihyo[i, junpai[0] / 10] && p_setteihyo[i, junpai[3] / 10]) || (junpai[3] / 10 != junpai[6] / 10 && p_setteihyo[i, junpai[3] / 10] && p_setteihyo[i, junpai[6] / 10]) || (junpai[0] / 10 != junpai[6] / 10 && p_setteihyo[i, junpai[0] / 10] && p_setteihyo[i, junpai[6] / 10]))
                        {
                            y_yaku.Add(new string[,] { { "混 " + p_zokusei[i] }, { "2000" } });
                            henten[n] += 20;
                        }
                    }
                }
            }

            if (n == oya)
            {//親
                y_yaku.Add(new string[,] { { "親" }, { "×1.5" } });
                henten[n] = henten[n] * 1.5;
            }

            if (n == mawariban)
            {
                double sanbun = Math.Round(henten[n] / 3);
                for (int x = 0; x < 4; x++)
                {
                    if (x == mawariban)
                    {
                        henten[n] = sanbun * 3;
                        tokuten[n] += henten[n];
                    }
                    else
                    {
                        henten[x] -= sanbun;
                        tokuten[x] -= sanbun;
                    }
                }
            }
            else
            {
                tokuten[n] += henten[n];
                henten[mawariban] -= henten[n];
                tokuten[mawariban] -= henten[n];
            }
        }

        private void ponhantei()
        {
            for (int i = 0; i < 4; i++)
            {
                if (i != mawariban && pondarekara[i][1] == -1 && !reachflag[i])
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((tehai[i][j] / 10 == sutehai[mawariban][sutemax[mawariban] - 1] / 10 && (tehai[i][j + 1] / 10 == sutehai[mawariban][sutemax[mawariban] - 1] / 10) || (tehai[i][j] / 10 == sutehai[mawariban][sutemax[mawariban] - 1] / 10 && (tehai[i][1] == 90 || tehai[i][4] == 90 || tehai[i][7] == 90))))
                        {
                            ponflag[i] = true;
                            if (h_sankaid[i] != "-1")
                            {                               
                                ponjusinflag[i] = true;
                            }
                            else
                            {
                                ponjusinflag[i] = false;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}