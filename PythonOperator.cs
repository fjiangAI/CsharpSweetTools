using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// PythonOperator 的摘要说明
/// 这是Python操作类，未作修改。
/// </summary>
public class PythonOperator
{
    private string pythonCmd = "";
    public PythonOperator()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        pythonCmd  = ConfigurationManager.ConnectionStrings["pythonPath"].ConnectionString;
    }

    public string InvokePython(string cmd, string input, string output)
    {
        string res = "";
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = pythonCmd;// "python";
        p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
        p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
        p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
        p.StartInfo.CreateNoWindow = true; //不显示程序窗口
                                           //p.Start(); //启动程序

        p.StartInfo.Arguments = cmd; //@"f:\rdfx.py";// f:\input.txt f:\output.txt";
        p.StartInfo.Arguments += " " + input;
        p.StartInfo.Arguments += " " + output;

        p.Start(); //启动程序
        //获取cmd窗口的输出信息
        res = p.StandardOutput.ReadToEnd();
        p.WaitForExit();//等待程序执行完退出进程
        p.Close();
        return res;
    }
}