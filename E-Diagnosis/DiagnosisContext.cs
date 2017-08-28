using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Diagnosis
{

    public enum Category
    {
        中成药与西药,
        中药,
    }

    //public enum FeeType
    //{
    //    自费,
    //    公费,
    //    医保,
    //    其他,
    //    合医,
    //    生育保险,
    //    商业保险,
    //}

    //public enum Gender
    //{
    //    男,
    //    女,
    //}

    //public enum Marriage
    //{
    //    已婚,
    //    未婚,
    //    离婚,
    //    丧偶,
    //}

    //public enum Type
    //{
    //    初诊,
    //    复诊,
    //}

    //public enum Department
    //{
    //    儿科,
    //    内科,
    //    口腔科,
    //    外科,
    //    妇科,
    //    骨科,
    //}

    //public enum BloodType
    //{
    //    A型,
    //    B型,
    //    O型,
    //    AB型,
    //    Rh阴性型,
    //    MNSSU型,
    //    P型,
    //}

    //public enum Nation
    //{
    //    汉族,
    //    壮族,
    //    满族,
    //    回族,
    //    苗族,
    //    维吾尔族,
    //    土家族,
    //    彝族,
    //    蒙古族,
    //    藏族,
    //    布依族,
    //    侗族,
    //    瑶族,
    //    朝鲜族,
    //    白族,
    //    哈尼族,
    //    哈萨克族,
    //    黎族,
    //    傣族,
    //    畲族,
    //    傈僳族,
    //    仡佬族,
    //    东乡族,
    //    高山族,
    //    拉祜族,
    //    水族,
    //    佤族,
    //    纳西族,
    //    羌族,
    //    土族,
    //    仫佬族,
    //    锡伯族,
    //    柯尔克孜族,
    //    达斡尔族,
    //    景颇族,
    //    毛南族,
    //    撒拉族,
    //    布朗族,
    //    塔吉克族,
    //    阿昌族,
    //    普米族,
    //    鄂温克族,
    //    怒族,
    //    京族,
    //    基诺族,
    //    德昂族,
    //    保安族,
    //    俄罗斯族,
    //    裕固族,
    //    乌孜别克族,
    //    门巴族,
    //    鄂伦春族,
    //    独龙族,
    //    塔塔尔族,
    //    赫哲族,
    //    珞巴族,
    //}

    public class Medicine
    {
        public int id { get; set; }
        public Category category { get; set; }
        public string 名称 { get; set; }
        public string 规格 { get; set; }
        public decimal 价格 { get; set; }
    }

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
        public virtual ICollection<Record> records { get; set; }
    }

    public class Record
    {
        public int id { get; set; }
        public virtual Patient patient { get; set; }
        public bool finished { get; set; } = false;
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
        public virtual Prescription wprescription { get; set; }
        public virtual Prescription cprescription { get; set; }
    }

    public class Prescription
    {
        public Prescription()
        {
            this.items = new List<Item>();
        }
        public int id { get; set; }
        public Record record { get; set; }
        public int amount { get; set; } = 1;
        public decimal price { get; set; }
        public virtual ICollection<Item> items { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public virtual Prescription prescription { get; set; }
        public string 名称 { get; set; }
        public decimal 单价 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 小计 { get; set; }
    }

    public class Template
    {
        public Template()
        {
            this.items = new List<TemplateItem>();
        }
        public int id { get; set; }
        public virtual ICollection<TemplateItem> items { get; set; }
        public string 名称 { get; set; }
        public string 功用 { get; set; }
        public string 主治 { get; set; }
        public string 备注 { get; set; }
    }

    public class TemplateItem
    {
        public int id { get; set; }
        public virtual Template template { get; set; }
        public virtual Medicine medicine { get; set; }
        public int amount { get; set; }
    }

    public class DiagnosisContext:DbContext
    {
        public DbSet<Medicine> medicine_set { get; set; }
        public DbSet<Patient> patient_set { get; set; }
        public DbSet<Template> template_set { get; set; }
    }
}
