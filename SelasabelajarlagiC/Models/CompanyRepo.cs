using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace SelasabelajarlagiC.Models
{
    public class CompanyRepo
    {
        Database db;
        Result res = new Result();
        public CompanyRepo()
		{
            db = new Database("Dbconnection");
		}

        public List<Company> SearchCompany(string name)
		{
            //string text = string.Format("SELECT * FROM TB_M_COMPANY WHERE [NAME] = '${0}'", name);
            //return db.Fetch<Company>(text).ToList();
            return db.Fetch<Company>("SELECT * FROM TB_M_COMPANY WHERE [NAME] = '"+name+"'").ToList();
        }

        public List<Company> GetCompany(string id = null)
        {            
            if (id == null || id == "")
			{
                return db.Fetch<Company>("SELECT * FROM TB_M_COMPANY").ToList();
            }        
			else
			{
                return db.Fetch<Company>("SELECT * FROM TB_M_COMPANY WHERE [ID] ='"+ id +"'").ToList();
            }            
		}

        public Result PostData(Company c)
		{            
			try
			{
                res = db.Fetch<Result>(";exec TEST_insertCompany @0, @1, @2, @3", new object[] { c.mode, c.ID, c.NAME, c.DESCRIPTION })[0];

            }
            catch(Exception e)
			{
                res.MESSAGE = e.Message;
                res.STATUS = false;
			}
            return res;
            /*db.Execute("INSERT INTO TB_M_COMPANY([ID],[NAME],[DESCRIPTION],[CREATED_BY],[CREATED_DATE]) VALUES ('" + c.ID + "','" + c.NAME + "', '" + c.DESCRIPTION + "','system',GETDATE());");
            return true;*/
		}

        public Result DeleteData(string id)
		{
			/*db.Execute("DELETE FROM TB_M_COMPANY WHERE ID='" + id + "';");
            return true;*/
			try
			{
                res = db.Fetch<Result>(";exec TEST_DeleteCompany @0", new object[] { id })[0];
			}catch(Exception e)
			{
                res.MESSAGE = e.Message;
                res.STATUS = false;
			}
            return res;
		}

        public Result UpdateData(Company c)
		{
            //db.Fetch<Result>("UPDATE TB_M_COMPANY SET NAME='" + c.NAME + "' WHERE [ID]='" + c.ID + "'");
            try
            {
                res = db.Fetch<Result>(";exec TEST_UpdateCompany @0, @1, @2", new object[] { c.ID, c.NAME, c.DESCRIPTION})[0];
            }catch(Exception e)
			{
                res.MESSAGE = e.Message;
                res.STATUS = false;
			}           
            return res;
		}
    }
}