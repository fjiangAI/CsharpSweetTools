using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
/// <summary>
/// 数据库操作类，已检验。未做异常修改。
/// </summary>
public class DataBaseOpt
{
    public static string sip;
    public static string sConnection;
    public SqlConnection cn=null;
    private SqlDataAdapter ada;
    public DataSet ds;
    public DataRow dr;
    public SqlTransaction tran;

    //**********************************>> 基本数据库操作篇 <<**********************************//

    /// <summary>
    /// 打开数据库连接
    /// </summary>
    public void Open()
    {
        //sConnection = "Data Source=localhost;Initial Catalog=myTest;Integrated Security=True";
        sConnection = ConfigurationManager.ConnectionStrings["sqlServerData"].ConnectionString;
        if (cn == null)
        {
            cn = new SqlConnection(sConnection);
            cn.Open();
        }
		else if(cn.State==ConnectionState.Closed)
        {
            cn.Open();
        }
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public void Close()
    {
        if (cn != null)
            cn.Close();
        cn = null;
    }

    /// <summary>
    /// 返回一个SqlParameter实例
    /// </summary>
    /// <param name="ParamName"></param>
    /// <param name="Value"></param>
    /// <returns></returns>
    public SqlParameter MakeParam(string ParamName, object Value)//参数名字和参数值
    {
        return new SqlParameter(ParamName, Value);
    }

    /// <summary>
    /// 调用存储过程创建一个SqlCommand对象
    /// </summary>
    /// <param name="procName">存储过程</param>
    /// <param name="prams">给存储过程传递传输SqlParameter对象</param>
    /// <returns></returns>
    private SqlCommand CreateCommand(string procName, SqlParameter[] prams)//存储过程名字和参数数组 
    {
        Open();
        //SqlCommand cmd = new SqlCommand(procName, cn);//存储过程名和数据库连接
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;//指明SqlCommand类型是存储过程
        cmd.CommandText = procName;
        cmd.Connection = cn;
        if (prams != null) //参数数组不为空
        {
            foreach (SqlParameter parameter in prams)
                cmd.Parameters.Add(parameter);
        }

        /*cmd.Parameters.Add(
            new SqlParameter("ReturnValue", SqlDbType.Int, 4,
            ParameterDirection.ReturnValue, false, 0, 0,
            string.Empty, DataRowVersion.Default, null));*/
        return cmd;//返回创建好的SqlCommand对象
    }

    /// <summary>
    /// 执行存储过程
    /// </summary>
    /// <param name="procName">存储过程名称</param>
    /// <param name="prams">给存储过程传递传输SqlParameter对象</param>
    /// <returns></returns>
    public void RunProc(string procName, SqlParameter[] prams)
    {
        SqlCommand cmd = CreateCommand(procName, prams);//需要存储过程名称和参数数组
        cmd.ExecuteNonQuery();//执行SqlCommand命令
        this.Close();//释放与数据库的连接
    }

    /// <summary>
    /// 执行存储过程
    /// </summary>
    /// <param name="procName">存储过程名称</param>
    /// <param name="prams">给存储过程传递传输SqlParameter对象</param>
    /// <param name="dataReader">输出一个DataReader对象</param>
    public void RunProc(string procName, SqlParameter[] prams, out SqlDataReader dataReader)
    {
        SqlCommand cmd = CreateCommand(procName, prams);
        dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
    }

    /// <summary>
    /// 获得DataSet对象
    /// </summary>
    /// <param name="str_Sql">SQL语句</param>
    /// <returns></returns>
    public DataSet GetDs(string str_Sql)//通过一条Sql语句获得一个数据集ds并返回之
    {
        Open();
        SqlDataAdapter Ada = new SqlDataAdapter(str_Sql, cn);
        DataSet ds = new DataSet();
        Ada.Fill(ds);
        cn.Close();
        return ds;
    }
    /// <summary>
    /// 获得DataSet对象
    /// </summary>
    /// <param name="tablename">内存表ID</param>
    /// <param name="str_Sql">SQL语句</param>
    /// <returns></returns>
    public DataSet GetDs(string tablename, string str_Sql)
    {
        Open();
        SqlDataAdapter Ada = new SqlDataAdapter(str_Sql, cn);
        DataSet ds = new DataSet();
        Ada.Fill(ds, tablename);
        cn.Close();
        return ds;
    }
    /// <summary>
    /// 获得DataTable对象
    /// </summary>
    /// <param name="str_Sql">SQL语句</param>
    /// <returns></returns>
    public DataTable GetTable(string str_Sql)
    {
        return GetDs(str_Sql).Tables[0];
    }
    /// <summary>
    /// 获得DataTable对象
    /// </summary>
    /// <param name="tablename">内存表ID</param>
    /// <param name="str_Sql">SQL语句</param>
    /// <returns></returns>
    public DataTable GetTable(string tablename, string str_Sql)
    {
        return GetDs(str_Sql).Tables[tablename];
    }

    public SqlDataReader GetDataReader(string str_Sql)
    {

            Open();
            SqlCommand cmd = new SqlCommand(str_Sql, cn);
            SqlDataReader dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            return dataReader;
            //如果关闭关联的DataReader对象，则关联的connection也将关闭
        
    }
    /// <summary>
    /// 执行Sql语句
    /// </summary>
    /// <param name="str_Sql">Sql语句</param>
    public void RunSql(string str_Sql)
    {
        Open();
        SqlCommand cmd = new SqlCommand(str_Sql, cn);
        cmd.ExecuteNonQuery();
        cn.Close();
    }



    //**********************************>> 数据库操作篇(包括添加Add,修改Edit,删除Del,详细Detail) <<**********************************//

    /// <summary>
    /// 读出详细信息赋给相应控件以及预先定义好的变量,控件包括Label,Text,RichTextBox,Calendar
    /// </summary>
    /// <param name="fileds">字段数组,比如：string[] fileds={"title","content","Derivation","Dates"};</param>
    /// <param name="ctrls">控件数组，比如：Control[] ctrls={txt_Title,txt_Content,txt_Derivation,cdr_Dates};</param>
    /// <param name="ref_fileds">ref输出字段数组</param>
    /// <param name="str_Sql">SQL语句</param>
    public void Detail(string[] fileds, Control[] ctrls, string[] ref_fileds, ref string[] ref_ctrls, string str_Sql)
    {
        SqlDataReader dr = GetDataReader(str_Sql);
        int i = 0;
        while (dr.Read())
        {
            foreach (Control ctrl in ctrls)
            {
                if (ctrl is TextBox)
                {
                    try
                    {
                        if (GetStrAppearTime(dr[fileds[i]].ToString(), "-") == 2)
                        {
                            ((TextBox)ctrl).Text = Convert.ToDateTime(dr[fileds[i]].ToString()).ToShortDateString();
                        }
                        else
                        {
                            ((TextBox)ctrl).Text = dr[fileds[i]].ToString();
                        }

                    }
                    catch
                    {
                        ((TextBox)ctrl).Text = dr[fileds[i]].ToString();

                    }
                }
                if (ctrl is Label)
                {
                    try
                    {
                        ((Label)ctrl).Text = Convert.ToDateTime(dr[fileds[i]].ToString()).ToShortDateString();

                    }
                    catch
                    {
                        ((Label)ctrl).Text = dr[fileds[i]].ToString();

                    }
                }

                i = i + 1;
            }
            for (int j = 0; j < ref_fileds.Length; j++)
            {
                ref_ctrls[j] = dr[ref_fileds[j]].ToString();
            }
            break;
        }
        dr.Close();
    }

    /// <summary>
    /// 读出详细信息赋给预先定义好的变量数组
    /// </summary>
    /// <param name="ref_fileds">字段数组,比如：string[] ref_fileds={"name","age"}</param>
    /// <param name="ref_Variables">变量数组,string[] ref_Variables={"",""}</param>
    /// <param name="str_Sql">SQL语句</param>
    public void Detail(string[] ref_fileds, ref string[] ref_Variables, string str_Sql)
    {
        SqlDataReader dr = GetDataReader(str_Sql);
        while (dr.Read())
        {
            for (int j = 0; j < ref_fileds.Length; j++)
            {
                ref_Variables[j] = dr[ref_fileds[j]].ToString();
            }
            break;
        }
        dr.Close();
    }

    /// <summary>
    /// 获得表记录数
    /// </summary>
    /// <param name="table_name">表名或者表名+条件,GetRsCount("t_user where id="+Request["id"])</param>
    /// <returns></returns>
    public int GetRsCount(string table_name)
    {
        string strSql;
        int intCount;
        Open();
        strSql = "select count(*) from " + table_name;
        SqlCommand cmd = new SqlCommand(strSql, cn);
        intCount = (int)cmd.ExecuteScalar();//strSql的查找结果是一个数据集
        cn.Close();
        return intCount;
    }
    /// <summary>
    /// 获得对应表数据集特定列的字符串集合并返回-BB
    /// </summary>
    /// <param name="table_name">字段名、表名</param>
    /// <returns>数据集</returns>
    /// 
    public DataSet GetTdsx(string sx_name, string table_name, string tj)
    {
        string strSql;
        if (tj.Trim() == "")
            strSql = "select " + sx_name + " from " + table_name;

        else
            strSql = "select " + sx_name + " from " + table_name + " where " + tj;
        //string connstr=ConfigurationSettings.AppSettings["sConnection"];
        SqlConnection objConnection = new SqlConnection(sConnection);
        SqlDataAdapter adapter = new SqlDataAdapter(strSql, objConnection);

        DataSet ds = new DataSet();
        adapter.Fill(ds, "table");
        return ds;
    }
    /// <summary>
    /// 获得单个int类型字段总和
    /// </summary>
    /// <param name="field">字段</param>
    /// <param name="table_name">表名或者表名+条件,GetFiledSum("id","t_user where id="+Request["id"])</param>
    /// <returns></returns>
    public string GetFiledSum(string field, string table_name)
    {
        string SumValue;
        Open();
        SqlCommand cmd = new SqlCommand("select sum(" + field + ") as s from " + table_name, cn);
        SumValue = cmd.ExecuteScalar().ToString();
        cn.Close();
        return SumValue;
    }
    public string GetFiledSum(string field)//字符串field应该是一个sql语句
    {
        string SumValue;
        Open();
        SqlCommand cmd = new SqlCommand(field, cn);
        SumValue = cmd.ExecuteScalar().ToString();
        cn.Close();
        return SumValue;
    }
    /// <summary>
    /// 获得单个字段值
    /// </summary>
    /// <param name="str_Sql">Sql语句</param>
    /// <returns></returns>
    public string GetFiledValue(string str_Sql)//通过一个Sql语句获取相关的属性域的值的函数
    {
        string str;
        Open();
        SqlCommand cmd = new SqlCommand(str_Sql, cn);
        try
        {
            str = cmd.ExecuteScalar().ToString();//返回查询结果集中的第一行第一列，忽略其他行和列
        }
        catch
        {
            return "";//sting.empty
        }
        cn.Close();
        return str;
    }

    /// <summary>
    /// 获得表记录数
    /// </summary>
    /// <param name="table_name">表名或者表名+条件,GetRsCount("t_user where id="+Request["id"])</param>
    /// <returns></returns>
    public int GetMaxId(string filed, string table_name)
    {
        string strSql;
        int intCount;
        Open();
        //用convert转化为int，因为许多field是string类型
        strSql = "select max(convert(int," + filed + ")) from " + table_name;
        SqlCommand cmd = new SqlCommand(strSql, cn);
        object obj = cmd.ExecuteScalar();
        if (obj == System.DBNull.Value)
        {
            intCount = 1;
        }
        else
        {
            intCount = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
        }
        cn.Close();
        return intCount;
    }

    /// <summary>
    /// 通过SqlCommandBuilder对象增加数据库记录
    /// </summary>
    /// <param name="sql">Select-SQL语句</param>
    public void Builder(string str_Sql)
    {
        Open();
        ada = new SqlDataAdapter(str_Sql, cn);
        SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(ada);
        ds = new DataSet();
        ada.Fill(ds);
        dr = ds.Tables[0].NewRow();
    }
    /// <summary>
    /// 关闭SqlCommandBuilder对象
    /// </summary>
    public void BuilderClose()
    {
        ds.Tables[0].Rows.Add(dr);
        ada.Update(ds);	 //　更新数据库					
        cn.Close(); // 关闭数据库
        ds.Clear(); // 清空DataSet对象
    }
    /// <summary>
    /// 通过SqlCommandBuilder对象修改数据库记录
    /// </summary>
    /// <param name="sql">Select-SQL语句</param>
    public void BuilderEdit(string str_Sql)
    {
        Open();
        ada = new SqlDataAdapter(str_Sql, cn);
        SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(ada);
        ds = new DataSet();
        ada.Fill(ds);
        dr = ds.Tables[0].Rows[0];
    }
    /// <summary>
    /// 关闭SqlCommandBuilder对象
    /// </summary>
    public void BuilderEditClose()
    {
        ada.Update(ds);	 //　更新数据库					
        cn.Close(); // 关闭数据库
        ds.Clear(); // 清空DataSet对象
    }

    public bool Exists(string tableName, string columnName1, string columnValue1, string columnName2, string columnValue2, string columnName3, string columnValue3)
    {
        Open();
        string sql = "";
        sql = "select count(*) from " + tableName + " where " + columnName1 + "='" + columnValue1 + "'";
        if (columnValue2 != "")
        {
            sql += " and " + columnName2 + "='" + columnValue2 + "'";
        }
        if (columnValue3 != "")
        {
            sql += " and " + columnName3 + "='" + columnValue3 + "'";
        }
        SqlCommand cmd = new SqlCommand(sql, cn);
        int intCount = (int)cmd.ExecuteScalar();//strSql的查找结果是一个数据集
        cn.Close();
        return (intCount>0);
    }

    public bool ExistsRole(string tableName, string judgeValue, string judgeName)
    {
        Open();
        string sql = "select count(*) from " + tableName + " where " + judgeName + "='" + judgeValue + "'";
        SqlCommand cmd = new SqlCommand(sql, cn);
        int intCount = (int)cmd.ExecuteScalar();//strSql的查找结果是一个数据集
        cn.Close();
        return (intCount > 0);
    }

    public bool intest(string columnName, string columnValue, string tableName)
    {
        cn.Open();
        string sql = "select intest from " + tableName + " where " + columnName + "='" + columnName + "'";
        SqlCommand cmd = new SqlCommand(sql, cn);
        SqlDataReader dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        dataReader.Read();
        int flag = Convert.ToInt32(dataReader["intest"].ToString());
        dataReader.Close();
        return (flag == 1);
    }

    //**********************************>> 事务处理篇 <<**********************************//

    /// <summary>
    /// 开始事务
    /// </summary>
    public void TranBegin()
    {
        Open();
        tran = cn.BeginTransaction();
    }
    /// <summary>
    /// 结束事务
    /// </summary>
    public void TranEnd()
    {
        tran.Commit(); // 结束事务
        if (cn != null)
        {
            cn.Dispose(); // 关闭数据库
            cn.Close(); // 关系数据库
        }
        if (ds != null)
        {
            ds.Clear();
        }
    }

    public SqlDataReader TranGetDataReader(string str_Sql)
    {
        Open();
        SqlCommand cmd = new SqlCommand(str_Sql, cn);
        SqlDataReader dr = cmd.ExecuteReader();
        return dr;
    }

    public DataSet TranGetDs(string str_Sql)
    {
        Open();
        SqlDataAdapter Ada = new SqlDataAdapter(str_Sql, cn);
        Ada.SelectCommand.Transaction = tran;
        DataSet ds = new DataSet();
        Ada.Fill(ds);
        //cn.Close();
        return ds;
    }

    public DataTable TranGetTable(string str_Sql)
    {
        return TranGetDs(str_Sql).Tables[0];
    }


    /// <summary>
    /// 获得表记录数
    /// </summary>
    /// <param name="table_name">表名或者表名+条件,GetRsCount("t_user where id="+Request["id"])</param>
    /// <returns></returns>
    public int TranGetRsCount(string table_name)
    {
        string strSql;
        int intCount;
        strSql = "select count(*) from " + table_name;
        SqlCommand cmd = new SqlCommand(strSql, cn);
        cmd.Transaction = tran;
        intCount = (int)cmd.ExecuteScalar();
        return intCount;
    }

    /// <summary>
    /// 获得单个int类型字段总和
    /// </summary>
    /// <param name="field">字段</param>
    /// <param name="table_name">表名或者表名+条件,GetFiledSum("id","t_user where id="+Request["id"])</param>
    /// <returns></returns>
    public string TranGetFiledSum(string field, string table_name)
    {
        string SumValue;
        SqlCommand cmd = new SqlCommand("select sum(" + field + ") as s from " + table_name, cn);
        cmd.Transaction = tran;
        SumValue = cmd.ExecuteScalar().ToString();
        return SumValue;
    }

    /// <summary>
    /// 获得单个字段值
    /// </summary>
    /// <param name="str_Sql">Sql语句</param>
    /// <returns></returns>
    public string TranGetFiledValue(string str_Sql)
    {
        string str;
        SqlCommand cmd = new SqlCommand(str_Sql, cn);
        cmd.Transaction = tran;
        str = cmd.ExecuteScalar().ToString();
        return str;
    }

    /// <summary>
    /// 执行Sql语句
    /// </summary>
    /// <param name="str_Sql">Sql语句</param>
    public void TranRunSql(string str_Sql)
    {
        SqlCommand cmd = new SqlCommand(str_Sql, cn);
        cmd.Transaction = tran;
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// 通过SqlCommandBuilder对象增加数据库记录
    /// </summary>
    /// <param name="sql">Select-SQL语句</param>
    public void TranBuilder(string str_Sql)
    {
        ada = new SqlDataAdapter(str_Sql, cn);
        ada.SelectCommand.Transaction = tran;
        SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(ada);
        ds = new DataSet();
        ada.Fill(ds);
        dr = ds.Tables[0].NewRow();//创建与该表有相同架构的新的System.Data.DataRow
    }
    /// <summary>
    /// 关闭SqlCommandBuilder对象
    /// </summary>
    public void TranBuilderClose()
    {
        ds.Tables[0].Rows.Add(dr);//将指定的System.Data.DataRow添加到System.Data.DataRowCollection对象中
        ada.Update(ds);	 //　更新数据库					
    }
    /// <summary>
    /// 通过SqlCommandBuilder对象修改数据库记录
    /// </summary>
    /// <param name="sql">Select-SQL语句</param>
    public void TranBuilderEdit(string str_Sql)
    {
        ada = new SqlDataAdapter(str_Sql, cn);
        ada.SelectCommand.Transaction = tran;
        SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(ada);
        ds = new DataSet();
        ada.Fill(ds);
        dr = ds.Tables[0].Rows[0];
    }
    /// <summary>
    /// 关闭SqlCommandBuilder对象
    /// </summary>
    public void TranBuilderEditClose()
    {
        ada.Update(ds);	 //　更新数据库					
    }
    /// <summary>
    /// 事务回滚
    /// </summary>
    public void TranRollback()
    {
        tran.Rollback(); // 数据库回滚
        cn.Dispose(); // 关闭数据库
        cn.Close(); // 关系数据库
    }

    //**********************************>> 控件操作篇 <<**********************************//
    /// <summary>
    /// 判断控件Text是否为空
    /// </summary>
    /// <param name="error">为空时的出错信息</param>
    /// <param name="ctrl">TextBox控件id值</param>
    /// <return>为空false, 否则true</return>


    public int GetStrAppearTime(string strOriginal, string strSymbol)
    {
        return strOriginal.Length - strOriginal.Replace(strSymbol, "").Length;
    }

    public int GetSingleCount(int courseid)//从表selectQuestion中获得单项选择题的个数
    {
        return this.GetRsCount(" selectQuestion WHERE type='0' and courseID='" + courseid + "'");
    }

    public int GetMultiCount(int courseid)
    {
        return this.GetRsCount(" selectQuestion WHERE type='1' and courseID='" + courseid + "'");//从表selectQuestion中获得多项选择题的个数
    }

    public String GetHtmlFormat(string strContent)
    {
        string val = strContent;
        if (val == null)
        {
            val = "";
        }
        //val=val.Replace("\r","<br>");
        Char a = Convert.ToChar(13);
        Char b = Convert.ToChar(10);
        String enter = a.ToString() + b.ToString();//enter对应为文本框里的enter键

        val = val.Replace(enter, "<br>");
        val = val.Replace(" ", "&nbsp;");

        val = val.Replace("'", "''");
        return val;
    }

    public String GetTxtFormat(String strContent)
    {
        String val = strContent;
        if (val == null)
        {
            return "";
        }

        Char a = Convert.ToChar(13);
        Char b = Convert.ToChar(10);
        String enter = a.ToString() + b.ToString();//enter对应为文本框里的enter键

        val = val.Replace("&nbsp;", " ");
        val = val.Replace("<br>", enter);

        return val;
    }

}
