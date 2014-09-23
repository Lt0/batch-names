using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace 批量重命名
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox5.Checked = true;

            
        }
       
        string[] oldfilelist;//包含文件夹的文件列表
        string[] nodirlist;//不包含文件夹的文件列表
        string[] dirlist;//文件夹列表

        //浏览按钮事件
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Clear();
                DialogResult dir = folderBrowserDialog1.ShowDialog();
                string thepath = folderBrowserDialog1.SelectedPath;
                textBox1.Text = thepath;
                //获取所有项目列表并打印
                try
                {
                    oldfilelist = Directory.GetFileSystemEntries(textBox1.Text);
                    textBox2.AppendText("找到指定路径以下项目：\n");
                    int sum1 = 0;
                    foreach (string str in oldfilelist)
                    {
                        textBox2.AppendText(str + "\n");
                        sum1++;
                    }

                    textBox2.AppendText("--------------------------------------------------------------------\n");

                    //获取文件夹列表并打印
                    dirlist = Directory.GetDirectories(textBox1.Text);
                    int sum2 = 0;
                    textBox2.AppendText("找到指定路径以下文件夹：\n");
                    foreach (string str in dirlist)
                    {
                        textBox2.AppendText(str + "\n");
                        sum2++;
                    }
                    textBox2.AppendText("--------------------------------------------------------------------\n");

                    //获取非文件夹列表并打印
                    nodirlist = getnodirlist(textBox1.Text);
                    int sum3 = 0;
                    textBox2.AppendText("找到指定路径以下非文件夹项目：\n");
                    foreach (string str in nodirlist)
                    {
                        textBox2.AppendText(str + "\n");
                        sum3++;
                    }

                    textBox2.AppendText("指定路径下一共" + sum1 + "个项目！\n");
                    textBox2.AppendText(sum2 + "个文件夹！\n");
                    textBox2.AppendText(sum3 + "个非文件夹项目！\n");
                    textBox2.AppendText("--------------------------------------------------------------------\n");
                }
                catch (ArgumentException)
                {

                }
            }
            catch
            { }
            
        }




        //重命名按钮事件
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox3.Text == "")
                {

                    Form2 fm = new Form2("新文件名不能为空！");

                    fm.ShowDialog();

                }
                else
                {
                    int littlefilenums = 0;
                    int littledirnums=0;
                    if (checkBox2.Checked == false)
                    {
                        string[] alldirlist = Directory.GetDirectories(textBox1.Text, "*", System.IO.SearchOption.AllDirectories);
                        int tmplittledirnums = 0;
                        int worknum = alldirlist.Length-1;
                        for (int i = worknum ; i >= 0; i--)
                        {
                            string[] tmpdirlist = Directory.GetDirectories(alldirlist[i]);
                            if (tmpdirlist != null)
                            {
                                string newname, dirindex;
                                for (int tmpi = 0; tmpi < tmpdirlist.Length; tmpi++)
                                {
                                    dirindex = getindex(tmpdirlist, tmpi);
                                    if (checkBox5.Checked == true)
                                    {
                                        newname = textBox3.Text + dirindex;

                                    }
                                    else
                                    {
                                        newname = dirindex + textBox3.Text;
                                    }
                                    newname = Path.Combine(alldirlist[i], newname);
                                    //alldirlist = null;
                                    if (tmpdirlist[tmpi] != newname)
                                    {
                                        Directory.Move(tmpdirlist[tmpi], newname);
                                    }
                                }
                                alldirlist = Directory.GetDirectories(textBox1.Text, "*", System.IO.SearchOption.AllDirectories);
                            }
                            tmplittledirnums = tmplittledirnums + tmpdirlist.Length;
                        }
                        littledirnums = tmplittledirnums;
                    }


                    if (checkBox3.Checked == true)
                    {
                        string[] alldirlist = Directory.GetDirectories(textBox1.Text, "*", System.IO.SearchOption.AllDirectories);
                        int filenums = 0;

                        foreach (string pathstr in alldirlist)
                        {
                            string[] tmpfilelist = Directory.GetFiles(pathstr, "*", System.IO.SearchOption.TopDirectoryOnly);
                            for (int tmpfileindex = 0; tmpfileindex < tmpfilelist.Length; tmpfileindex++)
                            {
                                //保留后缀名
                                if (checkBox1.Checked == true)
                                {
                                    string newname;
                                    //序号在后
                                    if (checkBox5.Checked == true)
                                    {
                                        newname = textBox3.Text + getindex(tmpfilelist, tmpfileindex) + Path.GetExtension(tmpfilelist[tmpfileindex]);
                                    }
                                    //序号在前
                                    else
                                    {
                                        newname = getindex(tmpfilelist, tmpfileindex) + textBox3.Text + Path.GetExtension(tmpfilelist[tmpfileindex]);

                                    }
                                    newname = Path.Combine(pathstr, newname);                                    

                                        File.Move(tmpfilelist[tmpfileindex], newname);
                                    
                                }
                                //不保留后缀名
                                else
                                {
                                    string newname;
                                    //序号在后
                                    if (checkBox5.Checked == true)
                                    {
                                        newname = textBox3.Text + getindex(tmpfilelist, tmpfileindex);
                                    }
                                    //序号在前
                                    else
                                    {
                                        newname = getindex(tmpfilelist, tmpfileindex);
                                    }

                                    newname = Path.Combine(pathstr, newname);
                                    File.Move(tmpfilelist[tmpfileindex], newname);
                                }
                            }
                            filenums = filenums + tmpfilelist.Length;
                        }
                        littlefilenums = filenums;

                    }

                    


                    //1、保留后缀名且忽略文件夹
                    if (checkBox1.Checked == true & checkBox2.Checked == true)
                    {
                        try
                        {

                            int index = 1;

                            string newname, nameextension, indexstr, onlyoldname, onlynewname;
                            for (int i = 0; i < nodirlist.Length; i++)
                            {
                                newname = Path.Combine(textBox1.Text, textBox3.Text);
                                nameextension = Path.GetExtension(nodirlist[i]);
                                indexstr = getindex(nodirlist, index);
                                if (checkBox5.Checked == true)
                                {
                                    newname = newname + indexstr + nameextension;
                                }
                                else
                                {
                                    newname = indexstr + textBox3.Text + nameextension;
                                    newname = Path.Combine(textBox1.Text , newname);
 
                                }
                                File.Move(nodirlist[i], newname);
                                onlyoldname = Path.GetFileName(nodirlist[i]);
                                onlynewname = Path.GetFileName(newname);
                                textBox2.AppendText("重命名文件\"" + onlyoldname + "\"为\"" + onlynewname + "\"成功！\n");
                                index++;
                            }
                            textBox2.AppendText("成功重命名指定路径下" + (index - 1) + "个文件！\n");
                            
                        }
                        catch (FileNotFoundException)
                        {

                            Form2 fm = new Form2("未找到文件！");
                            fm.ShowDialog();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Form2 fm = new Form2("路径不正确！");
                            fm.ShowDialog();
                        }
                        catch (IOException)
                        {
                            Form2 fm = new Form2("文件已存在！");
                            fm.ShowDialog();
                        }

                    }

                    //2、保留后缀名不忽略文件夹
                    if (checkBox1.Checked == true & checkBox2.Checked == false)
                    {
                        try
                        {
                            int dirnum = 0;
                            int fileindex = 1;
                            if (dirlist != null)
                            {

                                string newdirname, tmpstr, onlyoldname, onlynewname;
                                for (int i = 0; i < dirlist.Length; i++)
                                {
                                    tmpstr = getindex(oldfilelist, fileindex);
                                    if (checkBox5.Checked == true)
                                    {
                                        newdirname = Path.Combine(textBox1.Text, textBox3.Text);                                        
                                        newdirname = newdirname + tmpstr;

                                    }
                                    else
                                    {
                                        newdirname = tmpstr + textBox3.Text;
                                        newdirname = Path.Combine(textBox1.Text, newdirname);
                                    }
                                    Directory.Move(dirlist[i], newdirname);
                                    fileindex++;
                                    onlyoldname = Path.GetFileName(dirlist[i]);
                                    onlynewname = Path.GetFileName(newdirname);
                                    textBox2.AppendText("重命名文件夹\"" + onlyoldname + "\"为" + onlynewname + "\"成功！\n");

                                }
                                dirnum = fileindex - 1;

                            }
                            string newfilename, tmpfilestr, fileextension, onlyoldfilename, onlynewfilename;
                            for (int i = 0; i < nodirlist.Length; i++)
                            {
                                tmpfilestr = getindex(oldfilelist, fileindex);
                                fileextension = Path.GetExtension(nodirlist[i]);
                                if (checkBox5.Checked == true)
                                {
                                    newfilename = Path.Combine(textBox1.Text, textBox3.Text);
                                   
                                    newfilename = newfilename + tmpfilestr + fileextension;
                                }
                                else
                                {
                                    newfilename = tmpfilestr + textBox3.Text + fileextension;
                                    newfilename = Path.Combine(textBox1.Text, newfilename);
                                }
                                File.Move(nodirlist[i], newfilename);
                                fileindex++;
                                onlyoldfilename = Path.GetFileName(nodirlist[i]);
                                onlynewfilename = Path.GetFileName(newfilename);
                                textBox2.AppendText("重命名文件\"" + onlyoldfilename + "\"为\"" + onlynewfilename + "\"成功！\n");
                            }
                            textBox2.AppendText("一共成功重命名指定目录下" + dirnum + "个文件夹！\n");
                            textBox2.AppendText("一共成功重命名指定目录下" + (fileindex - dirnum - 1) + "个文件！\n");
                            textBox2.AppendText("共操作指定目录下项目：" + (fileindex - 1) + "个！\n");
                            textBox2.AppendText("--------------------------------------------------------------------\n");
                            
                        }
                        catch (FileNotFoundException)
                        {

                            Form2 fm = new Form2("未找到文件！");
                            fm.ShowDialog();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Form2 fm = new Form2("路径不正确！");
                            fm.ShowDialog();
                        }
                        catch (IOException)
                        {
                            Form2 fm = new Form2("文件已存在！");
                            fm.ShowDialog();
                        }

                    }

                    //3、不保留后缀名忽略文件夹
                    if (checkBox1.Checked == false & checkBox2.Checked == true)
                    {
                        try
                        {

                            int index = 1;

                            string newname, indexstr, onlyoldname, onlynewname;
                            for (int i = 0; i < nodirlist.Length; i++)
                            {
                                indexstr = getindex(nodirlist, index);
                                if (checkBox5.Checked == true)
                                {
                                    newname = Path.Combine(textBox1.Text, textBox3.Text);
                                    newname = newname + indexstr;
                                }
                                else
                                {
                                    newname = indexstr + textBox3.Text;
                                    newname = Path.Combine(textBox1.Text, newname);
                                }
                                File.Move(nodirlist[i], newname);
                                onlyoldname = Path.GetFileName(nodirlist[i]);
                                onlynewname = Path.GetFileName(newname);
                                textBox2.AppendText("重命名文件\"" + onlyoldname + "\"为\"" + onlynewname + "\"成功！\n");
                                index++;
                            }
                            textBox2.AppendText("成功重命名指定目录下" + index + "个文件！\n");
                            
                        }
                        catch (FileNotFoundException)
                        {
                            Form2 fm = new Form2("未找到文件！");
                            fm.ShowDialog();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Form2 fm = new Form2("路径不正确！");
                            fm.ShowDialog();
                        }
                        catch (IOException)
                        {
                            Form2 fm = new Form2("文件已存在！");
                            fm.ShowDialog();
                        }

                    }

                    //4、不保留后缀名不忽略文件夹
                    if (checkBox1.Checked == false & checkBox2.Checked == false)
                    {
                        try
                        {
                            int dirnum = 0;
                            int fileindex = 1;
                            if (dirlist != null)
                            {

                                string newdirname, tmpstr, onlyoldname, onlynewname;
                                for (int i = 0; i < dirlist.Length; i++)
                                {
                                    tmpstr = getindex(oldfilelist, fileindex);
                                    if (checkBox5.Checked == true)
                                    {
                                        newdirname = Path.Combine(textBox1.Text, textBox3.Text);
                                        newdirname = newdirname + tmpstr;
                                    }
                                    else
                                    {
                                        newdirname = tmpstr + textBox3.Text;
                                        newdirname = Path.Combine(textBox1.Text, newdirname);
                                    }
                                    Directory.Move(dirlist[i], newdirname);
                                    fileindex++;
                                    onlyoldname = Path.GetFileName(dirlist[i]);
                                    onlynewname = Path.GetFileName(newdirname);
                                    textBox2.AppendText("重命名文件夹\"" + onlyoldname + "\"为\"" + onlynewname + "\"成功！\n");

                                }
                                dirnum = fileindex;

                            }
                            string newfilename, tmpfilestr, onlyoldfilename, onlynewfilename;
                            for (int i = 0; i < nodirlist.Length; i++)
                            {
                                tmpfilestr = getindex(oldfilelist, fileindex);
                                if (checkBox5.Checked == true)
                                {
                                    newfilename = Path.Combine(textBox1.Text, textBox3.Text);
                                    newfilename = newfilename + tmpfilestr;
                                }
                                else
                                {
                                    newfilename = tmpfilestr + textBox3.Text;
                                    newfilename = Path.Combine(textBox1.Text, newfilename);
                                }
                                File.Move(nodirlist[i], newfilename);
                                fileindex++;
                                onlyoldfilename = Path.GetFileName(nodirlist[i]);
                                onlynewfilename = Path.GetFileName(newfilename);
                                textBox2.AppendText("重命名文件\"" + onlyoldfilename + "\"为\"" + onlynewfilename + "\"成功！\n");
                            }
                            textBox2.AppendText("一共成功重命名指定目录下" + dirnum + "个文件夹！\n");
                            textBox2.AppendText("一共成功重命名指定目录下" + (fileindex - dirnum) + "个文件！\n");
                            textBox2.AppendText("共操作指定目录下项目：" + fileindex + "个！\n");
                            textBox2.AppendText("--------------------------------------------------------------------\n");


                            if(checkBox3.Checked==true )
                            {
                                textBox2.AppendText("");
                            }

                        }
                        catch (FileNotFoundException)
                        {

                            Form2 fm = new Form2("未找到文件！");
                            fm.ShowDialog();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Form2 fm = new Form2("路径不正确！");
                            fm.ShowDialog();
                        }
                        catch (IOException)
                        {
                            Form2 fm = new Form2("文件已存在！");
                            fm.ShowDialog();
                        }

                    }

                    if (checkBox3.Checked == true&checkBox2.Checked==true )
                    {
                        textBox2.AppendText("重命名子文件夹文件" + littlefilenums + "个！\n");
                    }
                    if (checkBox3.Checked == true & checkBox2.Checked == false)
                    {
                        textBox2.AppendText("重命名子文件夹文件" + littlefilenums + "个！\n");
                        textBox2.AppendText("重命名子文件夹中的文件夹" + littledirnums + "个！\n");
                    }

                    //重命名父级目录
                if (checkBox6.Checked == true)
                {
                    string[] fullpathstrlist = textBox1.Text.Split('\\');
                    string pathstrlist = null;
                    for (int i = 0; i < (fullpathstrlist.Length - 1); i++)
                    {
                        pathstrlist = pathstrlist + fullpathstrlist[i] + "\\";
                    }
                    string newdirname = pathstrlist + textBox3.Text;
                    Directory.Move(textBox1.Text, newdirname);
                    textBox2.AppendText("重命名父级目录\"" + fullpathstrlist[(fullpathstrlist.Length - 1)] + "\"为\"" + textBox3.Text + "\"成功！\n");
                    textBox2.AppendText("--------------------------------------------------------------------\n");
                    textBox1.Text = newdirname;
                }



                //重命名完成后重新获取列表但不打印列表
                try
                {
                    //获取所有项目列表并打印
                    oldfilelist = Directory.GetFileSystemEntries(textBox1.Text);
                    //textBox2.AppendText("找到指定目录中以下项目：\n");
                    int sum1 = 0;
                    foreach (string str in oldfilelist)
                    {
                        //textBox2.AppendText(str + "\n");
                        sum1++;
                    }

                    //textBox2.AppendText("--------------------------------------------------------------------\n");

                    //获取文件夹列表并打印
                    dirlist = Directory.GetDirectories(textBox1.Text);
                    int sum2 = 0;
                    //textBox2.AppendText("找到指定目录中以下文件夹：\n");
                    foreach (string str in dirlist)
                    {
                        //textBox2.AppendText(str + "\n");
                        sum2++;
                    }
                    //textBox2.AppendText("--------------------------------------------------------------------\n");

                    //获取非文件夹列表并打印
                    nodirlist = getnodirlist(textBox1.Text);
                    int sum3 = 0;
                    //textBox2.AppendText("找到指定目录中以下非文件夹项目：\n");
                    foreach (string str in nodirlist)
                    {
                        //textBox2.AppendText(str + "\n");
                        sum3++;
                    }

                    textBox2.AppendText("当前指定目录里中一共" + sum1 + "个项目！\n");
                    textBox2.AppendText(sum2 + "个文件夹！\n");
                    textBox2.AppendText(sum3 + "个非文件夹项目！\n");
                    textBox2.AppendText("--------------------------------------------------------------------\n");
                }
                catch (ArgumentException)
                {
                    Form2 fm = new Form2("路径不合法！");
                    fm.ShowDialog();
                }
                catch (DirectoryNotFoundException)
                { }

                }
                
                }//try

                


                

            


            catch (NullReferenceException)
            {
                Form2 fm = new Form2("未获得文件列表，请刷新列表！");
                fm.ShowDialog();
            }
            catch (IOException)
            {
                string tmpstr = "对" + textBox3.Text + "的访问被拒绝\n请确保彻底关闭该目录及\n其所有子目录窗口！";
                Form2 fm = new Form2(tmpstr);
                fm.ShowDialog();
            }

        }




        //刷新按钮事件，刷新列表中的文件，并更新三个文件数列的内容
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //获取所有项目列表并打印
                oldfilelist = Directory.GetFileSystemEntries(textBox1.Text);
                textBox2.AppendText("找到指定目录中以下项目：\n");
                int sum1 = 0;
                foreach (string str in oldfilelist)
                {
                    textBox2.AppendText(str + "\n");
                    sum1++;
                }

                textBox2.AppendText("--------------------------------------------------------------------\n");

                //获取文件夹列表并打印
                dirlist = Directory.GetDirectories(textBox1.Text);
                int sum2 = 0;
                textBox2.AppendText("找到指定目录中以下文件夹：\n");
                foreach (string str in dirlist)
                {
                    textBox2.AppendText(str + "\n");
                    sum2++;
                }
                textBox2.AppendText("--------------------------------------------------------------------\n");

                //获取非文件夹列表并打印
                nodirlist = getnodirlist(textBox1.Text);
                int sum3 = 0;
                textBox2.AppendText("找到指定目录中以下非文件夹项目：\n");
                foreach (string str in nodirlist)
                {
                    textBox2.AppendText(str + "\n");
                    sum3++;
                }

                textBox2.AppendText("指定目录里中一共" + sum1 + "个项目！\n");
                textBox2.AppendText(sum2 + "个文件夹！\n");
                textBox2.AppendText(sum3 + "个非文件夹项目！\n");
                textBox2.AppendText("--------------------------------------------------------------------\n");
            }
            catch (ArgumentException)
            {
                Form2 fm = new Form2("路径不合法！");
                fm.ShowDialog();
            }
            catch (DirectoryNotFoundException)
            { }
        }


        //清空按钮事件
        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            oldfilelist = null;
            dirlist = null;
            nodirlist = null;
        }


        //通过textbox1中的文字检测路径是否存在
        private void textBox1_Validated(object sender, EventArgs e)
        {

                try
                {

                    //获取所有项目列表并打印
                    oldfilelist = Directory.GetFileSystemEntries(textBox1.Text);
                    textBox2.AppendText("找到指定目录中以下项目：\n");
                    int sum1 = 0;
                    foreach (string str in oldfilelist)
                    {
                        textBox2.AppendText(str + "\n");
                        sum1++;
                    }

                    textBox2.AppendText("--------------------------------------------------------------------\n");

                    //获取文件夹列表并打印
                    dirlist = Directory.GetDirectories(textBox1.Text);
                    int sum2 = 0;
                    textBox2.AppendText("找到指定目录中以下文件夹：\n");
                    foreach (string str in dirlist)
                    {
                        textBox2.AppendText(str + "\n");
                        sum2++;
                    }

                    textBox2.AppendText("--------------------------------------------------------------------\n");

                    //获取非文件夹列表并打印
                    nodirlist = getnodirlist(textBox1.Text);
                    int sum3 = 0;
                    textBox2.AppendText("找到指定目录中以下非文件夹项目：\n");
                    foreach (string str in nodirlist)
                    {
                        textBox2.AppendText(str + "\n");
                        sum3++;
                    }

                    textBox2.AppendText("指定目录中一共" + sum1 + "个项目！\n");
                    textBox2.AppendText(sum2 + "个文件夹！\n");
                    textBox2.AppendText(sum3 + "个非文件夹项目！\n");
                    textBox2.AppendText("--------------------------------------------------------------------\n");
                }
                catch (ArgumentException)
                {
                    //Form2 fm = new Form2("路径不合法！");
                    //fm.ShowDialog();

                }
                catch (DirectoryNotFoundException)
                {
                    Form2 fm = new Form2("路径不存在！\n");
                    fm.ShowDialog();
                }
            
            
        }

        //生成符合要求的文件名序列的方法
        public static string getindex(string[] filelist,int tmpnum)
        {
            int tmpnumber = filelist.Length;
            string tmpstr = tmpnumber.ToString();
            int numtime = tmpstr.Length;
            string filenamenum = tmpnum.ToString();
            switch (numtime)
            {
                case 2:
                    filenamenum = string.Format("{0:00}", tmpnum);
                    break;
                case 3:
                    filenamenum = string.Format("{0:000}", tmpnum);
                    break;
                case 4:
                    filenamenum = string.Format("{0:0000}", tmpnum);
                    break;
                case 5:
                    filenamenum = string.Format("{0:00000}", tmpnum);
                    break;
                case 6:
                    filenamenum = string.Format("{0:000000}", tmpnum);
                    break;
                case 7:
                    filenamenum = string.Format("{0:0000000}", tmpnum);
                    break;
                case 8:
                    filenamenum = string.Format("{0:00000000}", tmpnum);
                    break;

            }
            
            return filenamenum;
 
        }

        //获取目标路径下的非文件夹文件列表数列
        public static string[] getnodirlist(string thepath)
        {
            string[] alllist = Directory.GetFileSystemEntries(thepath);
            string[] dirlist = Directory.GetDirectories(thepath,"*");
            string[] nodirlist = new string[alllist.Length - dirlist.Length];
            bool isdir = false;
            int nodirlistindex = 0;
            for (int i = 0; i < alllist.Length; i++)
            {
                for (int n = 0; n < dirlist.Length; n++)
                {
                    if (alllist[i] == dirlist[n])
                    {
                        isdir = true;
                        continue;
                    }
                }
                if (isdir == false)
                {
                    nodirlist[nodirlistindex] = alllist[i];
                    nodirlistindex = nodirlistindex + 1;
                }
                else
                {
                    isdir = false;
                }
            }
            return nodirlist;
            
        }

        //退出按钮事件
        private void button6_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                checkBox4.Checked = false;
            }
            else
            {
                checkBox4.Checked = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                checkBox5.Checked = false;
            }
            else
            {
                checkBox5.Checked = true;
            }
        }


    
    }

}
