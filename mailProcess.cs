using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

/// <summary>
/// 发送邮件类
/// </summary>
public class mailProcess
{
    string emailAcount = "";
    string emailPassword = "";
    string sender = "";
    string host = "";
    int port = 0;
    string emailstr = "";
    public mailProcess(string emailAcount, string emailPassword, string sender, string host, int port)
    {
        this.emailAcount = emailAcount;
        this.emailPassword = emailPassword;
        this.sender = sender;
        this.host = host;
        this.port = port;
        this.emailstr = @"([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+";
    }

    public string sendMail(string title, string content, string recivers, string CCs)
    {
        string ret = "ok";
        try
        {
            MailMessage message = new MailMessage();
            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress(sender);
            message.From = fromAddr;
            Regex emailreg = new Regex(emailstr);
            //设置收件人
            foreach (string reciver in recivers.Split(';'))
            {
                //正确邮箱
                if (emailreg.IsMatch(reciver.Trim()))
                {
                    Console.WriteLine("收件人邮箱没问题！");
                    message.To.Add(reciver);
                }
                Console.WriteLine("收件人邮箱有问题！邮箱名为{0}",reciver);
            }
            //设置抄送人
            foreach (string CC in CCs.Split(';'))
            {
                //正确邮箱
                if (emailreg.IsMatch(CC.Trim()))
                {
                    Console.WriteLine("抄送人邮箱没问题！");
                    message.CC.Add(CC);
                }
                Console.WriteLine("抄送人邮箱有问题！邮箱名为{0}", CC);
            }
            //设置邮件标题
            message.Subject = title; //"Test";
            //设置邮件内容
            message.Body = content;
            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
            //SmtpClient client = new SmtpClient("smtp.suda.edu.cn", 25);
            SmtpClient client = new SmtpClient(host, port);
            //设置发送人的邮箱账号和密码
            client.Credentials = new NetworkCredential(emailAcount, emailPassword);
            ServicePointManager.ServerCertificateValidationCallback =
delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };
            //启用ssl,也就是安全发送
            client.EnableSsl = true;
            //发送邮件
            client.Send(message);
            Console.WriteLine("发送邮件完毕！");
        }
        catch (Exception ex)
        {
            ret = ex.Message;
        }
        return ret;
    }

}