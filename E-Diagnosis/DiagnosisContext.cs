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

    public class Medicine
    {
        public int id { get; set; }
        public Category category { get; set; }
        public string 名称 { get; set; }
        public string 规格 { get; set; }
        public decimal 价格 { get; set; }
        public override string ToString()
        {
            return this.名称;
        }
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
        public Medicine medicine { get; set; }
        public string 名称 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 单价 { get; set; }
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
        public string keywords { get; set; }
    }

    public class TemplateItem
    {
        public int id { get; set; }
        public virtual Template template { get; set; }
        public virtual Medicine 药品 { get; set; }
        public decimal 数量 { get; set; }
    }

    public class DiagnosisContext:DbContext
    {
        public DbSet<Medicine> medicine_set { get; set; }
        public DbSet<Patient> patient_set { get; set; }
        public DbSet<Template> template_set { get; set; }
    }
}
