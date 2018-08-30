using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SQLite;

namespace E_Diagnosis
{
    //在使用SQLite数据库后会出现一个bug，此处自定义了CHARINDEX函数解决了这一问题，一般情况下不必理会
    [SQLiteFunction(Arguments = 2, FuncType = FunctionType.Scalar, Name = "CHARINDEX")]
    class CustomCharIndex : SQLiteFunction
    {
        public override object Invoke(object[] args)//characters for the growth of
        {
            return args[1].ToString().IndexOf(args[0].ToString(), StringComparison.Ordinal) + 1;
        }
    }

    //定义药材的种类
    public enum Category
    {
        中成药,
        中药,
    }

    //药品表
    public class Medicine
    {
        public int id { get; set; }
        //药材种类
        public Category category { get; set; }
        public string 名称 { get; set; }
        public string 规格 { get; set; }
        public decimal 价格 { get; set; }
        //用于显示
        public override string ToString()
        {
            return this.名称;
        }
    }

    //病人表
    public class Patient
    {
        public Patient()
        {
            this.records = new List<Record>();
        }
        public int id { get; set; }
        public string 编号 { get; set; }
        public string 姓名 { get; set; }
        public string 性别 { get; set; }
        public decimal 年龄 { get; set; }
        public decimal 体重 { get; set; }
        public string 费别 { get; set; }
        public string 医保卡号 { get; set; }
        public string 民族 { get; set; }
        public string 血型 { get; set; }
        public string 籍贯 { get; set; }
        public string 职业 { get; set; }
        public string 婚姻 { get; set; }
        public string 身份证号 { get; set; }
        public string 住址 { get; set; }        
        public string 电话 { get; set; }
        public string 过敏史 { get; set; }
        //所有病历列表
        public virtual ICollection<Record> records { get; set; }
    }

    //病历表
    public class Record
    {
        public int id { get; set; }
        //对应病人（数据库中为外键）
        public virtual Patient patient { get; set; }
        public string 类型 { get; set; }
        public string 科别 { get; set; }
        public DateTime 就诊日期 { get; set; }
        public string 主诉 { get; set; }
        public string 现病史 { get; set; }
        public string 既往史 { get; set; }
        public string 个人史 { get; set; }
        public string 家族史 { get; set; }
        public string 月经及婚育史 { get; set; }
        public string 体格检查 { get; set; }
        public string 辅助检查 { get; set; }
        public string 临床诊断 { get; set; }
        public string 治疗意见 { get; set; }
        public string 说明 { get; set; }
        public string 医师 { get; set; }
        public string 调配 { get; set; }
        public string 审核 { get; set; }
        public string 核对 { get; set; }
        public string 发药 { get; set; }
        //西药处方
        public virtual Prescription wprescription { get; set; }
        //中药处方
        public virtual Prescription cprescription { get; set; }
        public string 服用方法 { get; set; }
    }

    //处方表
    public class Prescription
    {
        public Prescription()
        {
            this.items = new List<Item>();
        }
        public int id { get; set; }
        //对应处方
        public Record record { get; set; }
        //剂数（西药处方不使用）
        public int amount { get; set; } = 1;
        //处方价格（一剂）
        public decimal price { get; set; }
        //所包含的药品内容
        public virtual ICollection<Item> items { get; set; }
    }

    //处方中包含的药品表
    public class Item
    {
        public int id { get; set; }
        //对应处方
        public virtual Prescription prescription { get; set; }
        //对应药品
        public Medicine medicine { get; set; }
        //此处名称初始从药品库复制，但是可能会修改，修改后不影响药品库
        public string 名称 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 单价 { get; set; }
        public decimal 小计 { get; set; }
        public string 规格 { get; set; }
    }

    //模板表
    public class Template
    {
        public Template()
        {
            this.items = new List<TemplateItem>();
        }
        public int id { get; set; }
        //所包含的药品
        public virtual ICollection<TemplateItem> items { get; set; }
        public string 名称 { get; set; }
        public string 功用 { get; set; }
        public string 主治 { get; set; }
        public string 备注 { get; set; }
        //关键词
        public string keywords { get; set; }
    }

    //模板包含的药品表
    public class TemplateItem
    {
        public int id { get; set; }
        //对应模板
        public virtual Template template { get; set; }
        public virtual string 药品 { get; set; }
        public decimal 数量 { get; set; }
        public virtual Medicine medicine { get; set; }
        public string 规格 { get; set; }
    }

    public class DiagnosisContext:DbContext
    {
        //必须，调用了父类的构造函数，使用connection里的密码打开数据库
        public DiagnosisContext(DbConnection connection) : base(connection, true)
        {
        }
        //定义病人表、药品表、模板表，此处定义才可以直接访问，其它表通过这三个表间接访问
        public DbSet<Medicine> medicine_set { get; set; }
        public DbSet<Patient> patient_set { get; set; }
        public DbSet<Template> template_set { get; set; }
    }
}
