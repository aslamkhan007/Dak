using DakManSys;
using MVCGrid.Models;
using MVCGrid.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DakManSys.Entity;
using System.Reflection;
using DakManSys.Security;
using Security.Controllers;
using System.Globalization;
namespace DakManSys.App_Start
{
    public class MVCGridConfig
    {

        public static void RegisterGrids()
        {

            ColumnDefaults colDefauls = new ColumnDefaults()
            {
                EnableSorting = true
            };

            MVCGridDefinitionTable.Add("AcceptanceGrid", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithAdditionalQueryOptionNames("search")
                .AddColumns(cols =>
                {
                    cols.Add("Action").WithHeaderText("<input type='checkbox' data-mvcgrid-type='chkreply' data-mvcgrid-name='AcceptanceGrid' class='checkall'   />" + "Action")
                        .WithHtmlEncoding(false)
                        .WithVisibility(true, false)
                       .WithValueTemplate("<input type='checkbox' class='chbbox'  value='{Model.Jct_Dak_Register.Inward_No}'/>")
                        .WithSorting(false);
                    cols.Add("Inward_No").WithHeaderText("Inward No")
                        .WithHtmlEncoding(false)
                        .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                        .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                    cols.Add("Party_Name").WithHeaderText("Party Name")
                       .WithVisibility(true, false)
                       .WithValueExpression(p => p.partyName);
                    cols.Add("SUBJECT").WithHeaderText("Subject")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.SUBJECT);
                  
                    cols.Add("Reference_Date").WithHeaderText("Letter Date")
                       .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
                       .WithVisibility(visible: true, allowChangeVisibility: false)
                       .WithSorting(false);
                    cols.Add("Remarks").WithHeaderText("Remarks")
                        .WithVisibility(true, false)
                        .WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithValueTemplate("<input type='text' class='form-control txtRemarks' name='txtRemarks'/>");

                    cols.Add("Received_Status")
                       .WithVisibility(visible: true, allowChangeVisibility: false)
                       .WithHeaderText("Status")
                       .WithHtmlEncoding(false)
                       .WithSorting(false)
                       .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "<label class='label label-success'>Recieved</label>" : "<lable class='label label-danger'>Pending</label>")
                       .WithPlainTextValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "Recieved" : "Pending");

                    cols.Add("Reply_Action")

                        .WithHeaderText("Reply")
                        .WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithVisibility(true, false)
                        .WithValueTemplate("<input type='checkbox'   class='chbreply'  disabled='true'/>");

                    cols.Add("ReferenceNo")
                        .WithHeaderText("ReplyReference")
                        .WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithVisibility(true, false)
                        .WithValueTemplate("<input type='text' class='form-control txtreplyreference'  disabled='true'/>");
                    cols.Add("Replydate")
                        .WithHeaderText("Reply Date")
                        .WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithVisibility(true, false)
                        .WithValueTemplate("<input type='text' class='form-control txtreplydate'  disabled='true'>");
                    cols.Add("Entry_Date").WithHeaderText("Entry Date")
                      .WithValueExpression(p => p.Jct_Dak_Register.Created_On.HasValue ? p.Jct_Dak_Register.Created_On.Value.ToShortDateString() : "")
                      .WithVisibility(visible: true, allowChangeVisibility: false)
                      .WithSorting(false);
                })

                .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)

                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();

                    using (var db = new DRSEntities())
                    {
                        string CurrentUser = HttpContext.Current.User.Identity.Name.ToString();
                        string dept = db.jct_dak_role.Where(y=>y.empcode==CurrentUser).Select(x => x.Dept).FirstOrDefault();
                        db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                        var query = (from m in db.Jct_Dak_Register
                                     join q in
                                         (db.Jct_Dak_Party_Header.Select(x => new { x.Party_Code, x.Party_Name,x.Party_Type }).Distinct()) on m.Party_Code equals q.Party_Code
                                     where q.Party_Type == m.Party_Type && q.Party_Code == m.Party_Code 
                                     join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No into recive
                                     from b in recive
                                     where (m.Dept_Code==dept) && (b.Received_Status == false) && (m.Created_On > new DateTime(2016,03,31,23,0,0))
                                     select new DakManSys.ViewModel.GridViewModel
                                     {
                                         partyName=q.Party_Name,
                                         Jct_Dak_Register = m,
                                         Jct_Dak_Register_Recieved = b,


                                     }).AsQueryable();


                        result.TotalRecords = query.Count();
                        var globalSearch = options.GetAdditionalQueryOptionString("search");
                        if (result.TotalRecords > 0)
                        {
                            if (!String.IsNullOrWhiteSpace(globalSearch))
                            {
                                query = query.Where(p => p.partyName.Contains(globalSearch));
                            }
                        }
                        if (result.TotalRecords > 0)
                        {
                            if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                            {
                                switch (options.SortColumnName.ToLower())
                                {
                                    case "inward_no":
                                        query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                        break;

                                    case "party_name":
                                        query = query.OrderBy(p => p.partyName);
                                        break;

                                }
                            }
                        }
                        if (result.TotalRecords > 0)
                        {
                            if (options.GetLimitOffset().HasValue)
                            {
                                query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                            }
                        }
                        result.Items = query.ToList();
                    }
                    return result;
                })
            );


            //Monthly Report Grid
            MVCGridDefinitionTable.Add("MonthlyReport", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithAdditionalQueryOptionNames("search")
                .AddColumns(cols =>
                {
                    cols.Add("Inward_No")
                        .WithHeaderText("InwardNo")
                        .WithHtmlEncoding(false)
                        .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                        .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                    cols.Add("Party_Name").WithHeaderText("Party Name")
                       .WithVisibility(true, false)
                       .WithValueExpression(p => p.partyName);

                    cols.Add("City").WithHeaderText("City")
                  .WithVisibility(true, false)
                  .WithValueExpression(p => p.partyCity);

                    cols.Add("ReferenceNo").WithHeaderText("Refernce Number")
                       .WithVisibility(true, false)
                       .WithSorting(false)
                      .WithValueExpression(p => p.Jct_Dak_Register.ReferenceNo);
                                        cols.Add("Reference_Date").WithHeaderText("Letter Date")
                        .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
                        .WithVisibility(visible: true, allowChangeVisibility: false)
                        .WithSorting(false);

                                        cols.Add("DakService_Type").WithHeaderText("Dak Service")
                                           .WithVisibility(true, false)
                                           .WithSorting(false)
                                          .WithValueExpression(p => p.Jct_Dak_Register.DakService_Type);


                                        cols.Add("SUBJECT").WithHeaderText("Subject")
                                           .WithVisibility(true, false)
                                            .WithSorting(false)
                                           .WithValueExpression(p => p.Jct_Dak_Register.SUBJECT);
                    //cols.Add("Remarks").WithHeaderText("Remarks")
                    //    .WithVisibility(true, false)
                    //    .WithSorting(false)
                    //    .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Remarks);

                    //cols.Add("Dak Type").WithHeaderText("Dak Type")
                    //   .WithVisibility(true, false)
                    //   .WithSorting(false)
                    //  .WithValueExpression(p => p.Jct_Dak_Register.Dak_Type);

                   

                    cols.Add("ConcernedPersonals").WithHeaderText("ConcernedPersonals")
                        .WithVisibility(true,false)
                        .WithSorting(false)
                        .WithValueExpression(p=>p.Jct_Dak_Register.ConcernedPersonals);
                    cols.Add("Dept_Code").WithHeaderText("Department")
                        .WithVisibility(true,false)
                        .WithSorting(false)
                        .WithValueExpression(p=>p.Jct_Dak_Register.Department);
                    cols.Add("Received_Status")
                        .WithVisibility(visible: true, allowChangeVisibility: false)
                        .WithHeaderText("Status")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "<label class='label label-success'>Authorized</label>" : "<lable class='label label-danger'>Pending</label>")
                        .WithPlainTextValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "Authorized" : "Pending");


                    cols.Add("Created_On")
                        .WithVisibility(true, false)
                        .WithHeaderText("Entry Date")
                        .WithValueExpression(p => p.Jct_Dak_Register.Created_On.HasValue ? p.Jct_Dak_Register.Created_On.Value.ToShortDateString() : "")
                        .WithSorting(false);

               
                    //cols.Add("Replydate")
                    //    .WithHeaderText("Reply Date")
                    //    .WithVisibility(true, false)
                    //    .WithSorting(false)
                    //    .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.HasValue ? p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.Value.ToShortDateString() : "");
                })
                .WithAdditionalQueryOptionNames("startDate", "endDate")
                .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                //.WithAdditionalSetting("", false)
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    DateTime startDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("startDate")).Date;
                    DateTime endDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("endDate")).Date;



                    var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                    using (var db = new DRSEntities())
                    {


                        if (options.GetAdditionalQueryOptionString("startDate") == null)
                        {


                            startDate = DateTime.Today.AddDays(0);
                            endDate = DateTime.Today.AddDays(0);
                        }


                        var qry = (from p in db.Jct_Dak_Party_Detail_Address
                                   join c in db.Jct_Dak_Register on p.Party_Code equals c.Party_Code 
                                  where p.Party_Code==c.Party_Code && p.address_id == c.PartyAddress_id  &&  c.Party_Type==p.Party_Type                                   
                                   join w in db.Jct_Dak_Party_Header on c.Party_Code equals w.Party_Code
                                   where w.Party_Type == c.Party_Type &&  w.Party_Type==c.Party_Type  
                                
                                   select new
                                   {
                                       c.Party_Code,
                                       w.Party_Name,
                                       c.Party_Type,
                                       p.City,
                                       p.address_id

                                   }).Distinct();

                     

                        var query = (from m in db.Jct_Dak_Register

                                     join q in qry on m.Party_Code equals q.Party_Code where  m.Party_Type== q.Party_Type
                                     || m.Party_Type == null || q.Party_Type == null 

                                     join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No into recive
                                     from b in recive
                                     where (m.Created_On >= startDate) && (m.Created_On <= endDate)
                                     orderby m.Reference_Date descending
                                     select new DakManSys.ViewModel.GridViewModel
                                     {
                                         partyName=q.Party_Name,
                                         partyCity=q.City,
                                         Jct_Dak_Register = m,
                                         Jct_Dak_Register_Recieved = b,
                                     }).AsQueryable();

                        result.TotalRecords = query.Count();

                        var globalSearch = options.GetAdditionalQueryOptionString("search");
                        if (!String.IsNullOrWhiteSpace(globalSearch))
                        {
                            query = query.Where(p => p.Jct_Dak_Register.Inward_No.Contains(globalSearch) || p.partyName.Contains(globalSearch) || p.Jct_Dak_Register.ReferenceNo.Contains(globalSearch));
                        }
                        if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                        {
                            switch (options.SortColumnName.ToLower())
                            {
                                case "inward_no":
                                    query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                    break;
                                case "party_name":
                                    query = query.OrderBy(p => p.partyName);
                                    break;
                                case "received_status":
                                    query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                    break;
                            }
                        }
                        if (options.GetLimitOffset().HasValue)
                        {
                            query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                        }
                        result.Items = query.ToList();

                    }
                    return result;
                })
);

            //DepartmentWise Report
            MVCGridDefinitionTable.Add("DepartmentWiseReport", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithAdditionalQueryOptionNames("search")
                .AddColumns(cols =>
                {
                    cols.Add("Inward_No")
                        .WithHeaderText("InwardNo")
                        .WithHtmlEncoding(false)
                        .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                       .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);

                     
                    cols.Add("Party_Name").WithHeaderText("Party Name")
                       .WithVisibility(true, false)
                       .WithValueExpression(p => p.partyName);
                    cols.Add("SUBJECT").WithHeaderText("Subject")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.SUBJECT);
                    cols.Add("Reference_Date").WithHeaderText("Letter Date")
                        .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
                        .WithVisibility(visible: true, allowChangeVisibility: false)
                        .WithSorting(false);
                    cols.Add("Remarks").WithHeaderText("Remarks")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Remarks);
                    cols.Add("ConcernedPersonals").WithHeaderText("ConcernedPersonals")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.ConcernedPersonals);
                    cols.Add("Received_Status")
                      .WithVisibility(visible: true, allowChangeVisibility: false)
                      .WithHeaderText("Status")
                      .WithHtmlEncoding(false)
                      .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "<label class='label label-success'>Authorized</label>" : "<lable class='label label-danger'>Pending</label>")
                      .WithPlainTextValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "Authorized" : "Pending");
                    cols.Add("Created_On")
                       .WithVisibility(true, false)
                       .WithHeaderText("Authorized Date")
                       .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Created_On.HasValue ? p.Jct_Dak_Register_Recieved.Created_On.Value.ToShortDateString() : "")
                       .WithSorting(true);

                    cols.Add("ReferenceNo")
                        .WithHeaderText("ReplyReference")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceNo);
                    cols.Add("Replydate")
                        .WithHeaderText("Reply Date")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.HasValue ? p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.Value.ToShortDateString() : "");
                })
                .WithAdditionalQueryOptionNames("DeptCode")
                .WithSorting(true, "Created_On", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    string DeptCode = options.GetAdditionalQueryOptionString("DeptCode");
                    var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                    using (var db = new DRSEntities())
                    {

                        var qry = (from p in db.Jct_Dak_Party_Header
                                   select new
                                   {
                                       p.Party_Code,
                                       p.Party_Name,
                                       p.Party_Type

                                   }).Distinct();

                        var query = (from m in db.Jct_Dak_Register

                                     join q in qry on m.Party_Code equals q.Party_Code
                                     where q.Party_Type == m.Party_Type && q.Party_Code == m.Party_Code 
                                     join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No into recive
                                     from b in recive
                                     where m.Dept_Code == DeptCode
                                     
                                     select new DakManSys.ViewModel.GridViewModel
                                     { partyName=q.Party_Name,
                                         Jct_Dak_Register = m,
                                         Jct_Dak_Register_Recieved = b,
                                     }).AsQueryable();
                        result.TotalRecords = query.Count();


                        var globalSearch = options.GetAdditionalQueryOptionString("search");
                        if (!String.IsNullOrWhiteSpace(globalSearch))
                        {
                            //query = query.Where(p => p.partyName.Contains(globalSearch));
                            query = query.Where(p => p.Jct_Dak_Register.Inward_No.Contains(globalSearch) || p.partyName.Contains(globalSearch) );
                        }
                        if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                        {
                            switch (options.SortColumnName.ToLower())
                            {
                                case "inward_no":
                                    query = query.OrderByDescending(p => p.Jct_Dak_Register.Inward_No);
                                    break;
                                case "party_name":
                                    query = query.OrderBy(p => p.partyName);
                                    break;
                                case "received_status":
                                    query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                    break;
                                case "created_on":
                                    query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                    break;
                            }
                        }
                        if (options.GetLimitOffset().HasValue)
                        {
                            query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                        }
                        result.Items = query.ToList();
                    }
                    return result;
                })

           );


            //User Monthly Report
            MVCGridDefinitionTable.Add("UserMonthlyReport", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
               .WithAuthorizationType(AuthorizationType.AllowAnonymous)
               .WithAdditionalQueryOptionNames("search")
               .AddColumns(cols =>
               {
                   cols.Add("Inward_No")
                       .WithHeaderText("InwardNo")
                       .WithHtmlEncoding(false)
                       .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                       .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                   cols.Add("Party_Name").WithHeaderText("Party Name")
                      .WithVisibility(true, false)
                      .WithValueExpression(p => p.partyName);
                   cols.Add("SUBJECT").WithHeaderText("Subject")
                       .WithVisibility(true, false)
                       .WithValueExpression(p => p.Jct_Dak_Register.SUBJECT);
                   cols.Add("Reference_Date").WithHeaderText("Letter Date")
                       .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
                       .WithVisibility(visible: true, allowChangeVisibility: false)
                       .WithSorting(false);
                   cols.Add("Remarks").WithHeaderText("Remarks")
                       .WithVisibility(true, false)
                       .WithSorting(false)
                       .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Remarks);
                   cols.Add("Received_Status")
                       .WithVisibility(visible: true, allowChangeVisibility: false)
                       .WithHeaderText("Status")
                       .WithHtmlEncoding(false)
                       .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "<label class='label label-success'>Recieved</label>" : "<lable class='label label-danger'>Pending</label>")
                       .WithPlainTextValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "Authorized" : "Pending")
                       .WithSorting(true);


                   cols.Add("ReferenceNo")
                       .WithHeaderText("ReplyReference")
                       .WithVisibility(true, false)
                       .WithSorting(false)
                       .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceNo);
                   cols.Add("Replydate")
                       .WithHeaderText("Reply Date")
                       .WithVisibility(true, false)
                       .WithSorting(false)
                       .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.HasValue ? p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.Value.ToShortDateString() : "");


                   cols.Add("Created_On")
                      .WithVisibility(true, false)
                      .WithHeaderText("Entry Date")
                      .WithValueExpression(p => p.Jct_Dak_Register.Created_On.HasValue ? p.Jct_Dak_Register.Created_On.Value.ToShortDateString() : "")
                      .WithSorting(false);

                   

               })
               .WithAdditionalQueryOptionNames("startDate", "endDate")
               .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
               .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                //.WithAdditionalSetting("", false)
               .WithRetrieveDataMethod((context) =>
               {

                   var options = context.QueryOptions;
                   string CurrentUser = HttpContext.Current.User.Identity.Name;
                   DateTime startDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("startDate"));
                   DateTime endDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("endDate"));
                   string usercode = HttpContext.Current.User.Identity.Name.ToString();
                   var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                   using (var db = new DRSEntities())
                   {
                       db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                       if (options.GetAdditionalQueryOptionString("startDate") == null || options.GetAdditionalQueryOptionString("endDate") == null)
                       {
                           startDate = DateTime.Today.AddDays(0);
                           endDate = DateTime.Today.AddDays(0);
                       }

                       

                       string dept = db.jct_dak_role.Where(y => y.empcode == CurrentUser).Select(x => x.Dept).FirstOrDefault();

                       var qry = (from p in db.Jct_Dak_Party_Header
                                  select new
                                  {
                                      p.Party_Code,
                                      p.Party_Name,
                                      p.Party_Type
                                  }).Distinct();

                       var query = (from m in db.Jct_Dak_Register

                                    join q in qry on m.Party_Code equals q.Party_Code
                                    where q.Party_Type==m.Party_Type && q.Party_Code==m.Party_Code 
                                    join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No into recive
                                    from b in recive
                                    where (m.Created_On >= startDate) && (m.Created_On <= endDate) && (m.Dept_Code==dept)
                                    orderby m.Reference_Date descending
                                    select new DakManSys.ViewModel.GridViewModel
                                    {
                                        partyName=q.Party_Name,
                                        Jct_Dak_Register = m,
                                        Jct_Dak_Register_Recieved = b,
                                    }).AsQueryable();
                       result.TotalRecords = query.Count();
                       var globalSearch = options.GetAdditionalQueryOptionString("search");
                       if (!String.IsNullOrWhiteSpace(globalSearch))
                       {
                           
                           query = query.Where(p => p.Jct_Dak_Register.Inward_No.Contains(globalSearch) || p.partyName.Contains(globalSearch) || p.Jct_Dak_Register.ReferenceNo.Contains(globalSearch));
                       }
                       if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                       {
                           switch (options.SortColumnName.ToLower())
                           {
                               case "inward_no":
                                   query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                   break;
                               case "party_name":
                                   query = query.OrderBy(p => p.partyName);
                                   break;
                               case "received_status":
                                   query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                   break;
                           }
                       }
                       if (options.GetLimitOffset().HasValue)
                       {
                           query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                       }
                       result.Items = query.ToList();

                   }
                   return result;
               })
               );
            //--------------------------------------------//

            //-------------------------User Dak Recieved Report-----------------//
            MVCGridDefinitionTable.Add("UserDakRecievedReport", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
             .WithAuthorizationType(AuthorizationType.AllowAnonymous)
             .WithAdditionalQueryOptionNames("search")
             .AddColumns(cols =>
             {
                 cols.Add("Inward_No")
                     .WithHeaderText("InwardNo")
                     .WithHtmlEncoding(false)
                     .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                     .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                 cols.Add("Party_Name").WithHeaderText("Party Name")
                    .WithVisibility(true, false)
                    .WithValueExpression(p => p.partyName);
                 cols.Add("SUBJECT").WithHeaderText("Subject")
                     .WithVisibility(true, false)
                     .WithValueExpression(p => p.Jct_Dak_Register.SUBJECT);
                 cols.Add("Reference_Date").WithHeaderText("Letter Date")
                     .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
                     .WithVisibility(visible: true, allowChangeVisibility: false)
                     .WithSorting(false);
                 cols.Add("Remarks").WithHeaderText("Remarks")
                     .WithVisibility(true, false)
                     .WithSorting(false)
                     .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Remarks);
                 cols.Add("Received_Status")
                     .WithVisibility(visible: true, allowChangeVisibility: false)
                     .WithHeaderText("Status")
                     .WithHtmlEncoding(false)
                     .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "<label class='label label-success'>Recieved</label>" : "<lable class='label label-danger'>Pending</label>")
                     .WithPlainTextValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "Authorized" : "Pending")
                     .WithSorting(true);
                 cols.Add("Empcode")
                     .WithVisibility(visible: true, allowChangeVisibility: false)
                     .WithHeaderText("Recieved By")
                     .WithSorting(false)
                     .WithValueExpression(p => p.JCT_EMP_HOD.Hod_Name);
                 cols.Add("Created_On")
                     .WithHeaderText("Recieved Date")
                     .WithVisibility(visible: true, allowChangeVisibility: false)
                     .WithSorting(false)
                     .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Created_On.HasValue ? p.Jct_Dak_Register_Recieved.Created_On.Value.ToShortDateString() : "");
                 cols.Add("ReferenceNo")
                     .WithHeaderText("ReplyReference")
                     .WithVisibility(true, false)
                     .WithSorting(false)
                     .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceNo);
                 cols.Add("Replydate")
                     .WithHeaderText("Reply Date")
                     .WithVisibility(true, false)
                     .WithSorting(false)
                     .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.HasValue ? p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.Value.ToShortDateString() : "");
                 cols.Add("Entry_Date").WithHeaderText("Entry  Date")
                     .WithValueExpression(p => p.Jct_Dak_Register.Created_On.HasValue ? p.Jct_Dak_Register.Created_On.Value.ToShortDateString() : "")
                     .WithVisibility(visible: true, allowChangeVisibility: false)
                     .WithSorting(false);
             })
             .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
             .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                //.WithAdditionalSetting("", false)
             .WithRetrieveDataMethod((context) =>
             {
                 var options = context.QueryOptions;
                 string CurrentUser = HttpContext.Current.User.Identity.Name;
                 string usercode = HttpContext.Current.User.Identity.Name.ToString();
                 var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                 using (var db = new DRSEntities())
                 {
                     string dept = db.jct_dak_role.Where(y => y.empcode == CurrentUser).Select(x => x.Dept).FirstOrDefault();

                     var qry = (from p in db.Jct_Dak_Party_Header
                                select new
                                {
                                    p.Party_Code,
                                    p.Party_Name,
                                    p.Party_Type

                                }).Distinct();

                     var query = (from m in db.Jct_Dak_Register

                                  join q in qry on m.Party_Code equals q.Party_Code
                                  where q.Party_Type == m.Party_Type && q.Party_Code == m.Party_Code 
                                  join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No 
                                  join d in db.JCT_EMP_HOD on b.Empcode equals d.Hod_Code into recive
                                  from d in recive
                                  where (b.Received_Status==true) && (m.Dept_Code == dept) && (m.Created_On > new DateTime(2016,03,31,23,0,0))
                                  orderby m.Reference_Date descending
                                  select new DakManSys.ViewModel.GridViewModel
                                  {
                                      partyName = q.Party_Name,
                                      Jct_Dak_Register = m,
                                      JCT_EMP_HOD=d,
                                      Jct_Dak_Register_Recieved = b,
                                  }).AsQueryable();
                     result.TotalRecords = query.Count();
                     var globalSearch = options.GetAdditionalQueryOptionString("search");
                     if (!String.IsNullOrWhiteSpace(globalSearch))
                     {
                     //    query = query.Where(p => p.partyName.Contains(globalSearch));
                         query = query.Where(p => p.Jct_Dak_Register.Inward_No.Contains(globalSearch) || p.partyName.Contains(globalSearch) || p.Jct_Dak_Register.ReferenceNo.Contains(globalSearch));
                     }
                     if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                     {
                         switch (options.SortColumnName.ToLower())
                         {
                             case "inward_no":
                                 query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                 break;
                             case "party_name":
                                 query = query.OrderBy(p => p.partyName);
                                 break;
                             case "received_status":
                                 query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                 break;
                         }
                     }
                     if (options.GetLimitOffset().HasValue)
                     {
                         query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                     }
                     result.Items = query.ToList();

                 }
                 return result;
             })
             );


            //------------------------------------------------------------------//

            //-----------------BulkMailGrid----------------------------//
            MVCGridDefinitionTable.Add("SendMail", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
              .WithAuthorizationType(AuthorizationType.AllowAnonymous)
              //.WithAdditionalQueryOptionNames("search")
              .AddColumns(cols =>
              {
                  cols.Add("Inward_No")
                      .WithHeaderText("InwardNo")
                      .WithHtmlEncoding(false)
                      .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                      .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                  cols.Add("Hod_Name").WithHeaderText("To")
                     .WithVisibility(true, false)
                     .WithValueExpression(p => p.JCT_EMP_HOD.Hod_Name);
                  cols.Add("Reference_Date").WithHeaderText("Letter Date")
                      .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
                      .WithVisibility(visible: true, allowChangeVisibility: false)
                      .WithSorting(false);
                  cols.Add("Mail_Status").WithHeaderText("Mail Sent")
                      .WithVisibility(true, false)
                      .WithSorting(false)
                      .WithHtmlEncoding(false)
.WithValueExpression(p => p.Jct_Dak_Register.Email_Status ? "<label class='label label-success'>Recieved</label>" : "<lable class='label label-danger'>Pending</label>")
                       .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Email_Status ? "Sent" : "Pending");
              })
              .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
              .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                //.WithAdditionalSetting("", false)
              .WithRetrieveDataMethod((context) =>
              {

                  var options = context.QueryOptions;
                  var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                  using (var db = new DRSEntities())
                  {
                      db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                      var query = (from m in db.Jct_Dak_Register
                             join b in db.JCT_EMP_HOD on m.Hod_Code equals b.Hod_Code into recive
                                   from b in recive
                                   where m.Email_Status==false
                                   orderby m.Reference_Date descending
                                   select new DakManSys.ViewModel.GridViewModel
                                   {

                                       Jct_Dak_Register = m,
                                       JCT_EMP_HOD = b,
                                   }).AsQueryable();

                      result.TotalRecords = query.Count();
                      if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                      {
                          switch (options.SortColumnName.ToLower())
                          {
                              case "inward_no":
                                  query = query.OrderByDescending(p => p.Jct_Dak_Register.Inward_No);
                                  break;
                              case "party_name":
                                  query = query.OrderBy(p => p.partyName);
                                  break;
                              case "received_status":
                                  query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                  break;
                          }
                      }
                      if (options.GetLimitOffset().HasValue)
                      {
                          query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                      }
                   

                      result.Items = query.ToList();

                  }
                  return result;
              })
              );
            //--------------------------------------------------------//

            //-----------------BulkConcernedMailGrid----------------------------//
            MVCGridDefinitionTable.Add("SendConcernedMail", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
              .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                //.WithAdditionalQueryOptionNames("search")
              .AddColumns(cols =>
              {
                  cols.Add("Inward_No")
                      .WithHeaderText("InwardNo")
                      .WithHtmlEncoding(false)
                      .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                      .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                  cols.Add("ConcernedPersonals").WithHeaderText("To")
                     .WithVisibility(true, false)
                     .WithValueExpression(p => p.Jct_Dak_Register.ConcernedPersonals);
                  cols.Add("Created_On").WithHeaderText("Entry Date")
                      .WithValueExpression(p => p.Jct_Dak_Register.Created_On.HasValue ? p.Jct_Dak_Register.Created_On.Value.ToShortDateString() : "")
                      .WithVisibility(visible: true, allowChangeVisibility: false)
                      .WithSorting(false);
                  cols.Add("Email_Status").WithHeaderText("Mail Sent")
                      .WithVisibility(true, false)
                      .WithSorting(false)
                      .WithHtmlEncoding(false)
.WithValueExpression(p => Convert.ToBoolean(p.Jct_Dak_Register_Recieved.Email_Status) ? "<label class='label label-success'>Recieved</label>" : "<lable class='label label-danger'>Pending</label>")
                       .WithPlainTextValueExpression(p => Convert.ToBoolean(p.Jct_Dak_Register_Recieved.Email_Status) ? "Sent" : "Pending");
              })
              .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
              .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                //.WithAdditionalSetting("", false)
              .WithRetrieveDataMethod((context) =>
              {

                  var options = context.QueryOptions;
                  var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                  using (var db = new DRSEntities())
                  {
                      db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                      var query = (from m in db.Jct_Dak_Register_Recieved
                                   join c in db.Jct_Dak_Register on m.Inward_No equals c.Inward_No 

                                   where ( m.Email_Status == false && c.ConcernedPersonals!=null) && (m.Email_Status == false && c.ConcernedPersonals !="")
                                   select new DakManSys.ViewModel.GridViewModel
                                   {
                                       Jct_Dak_Register_Recieved=m,
                                       Jct_Dak_Register = c,
                                   }).AsQueryable();

                      result.TotalRecords = query.Count();
                      if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                      {
                          switch (options.SortColumnName.ToLower())
                          {
                              case "inward_no":
                                  query = query.OrderByDescending(p => p.Jct_Dak_Register.Inward_No);
                                  break;
                              case "party_name":
                                  query = query.OrderBy(p => p.partyName);
                                  break;
                              case "received_status":
                                  query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                  break;
                          }
                      }
                      if (options.GetLimitOffset().HasValue)
                      {
                          query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                      }


                      result.Items = query.ToList();

                  }
                  return result;
              })
              );
            //--------------------------------------------------------//
            //----------------------Cheque Report-------------------//
            MVCGridDefinitionTable.Add("DakTypeReport", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithAdditionalQueryOptionNames("search")
                .AddColumns(cols =>
                {
                    cols.Add("Inward_No")
                        .WithHeaderText("InwardNo")
                        .WithHtmlEncoding(false)
                        .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                        .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                    cols.Add("Party_Name").WithHeaderText("Party Name")
                      .WithVisibility(true, false)

                      .WithValueExpression(p => p.partyName);

                    cols.Add("ReferenceNo").WithHeaderText("Refernce Number")
                      .WithVisibility(true, false)
                      .WithSorting(false)
                     .WithValueExpression(p => p.Jct_Dak_Register.ReferenceNo);
                    
                    cols.Add("SUBJECT").WithHeaderText("Subject")
                        .WithVisibility(true, false)
                         .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.SUBJECT);
                    cols.Add("Cheque_No")
                        .WithHeaderText("Cheque Number")
                        .WithValueExpression(p => p.Jct_Dak_Register_Cheque.Cheque_No);

                    cols.Add("Reference_Date").WithHeaderText("Cheque Date")
                      .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
                      .WithVisibility(visible: true, allowChangeVisibility: false)
                      .WithSorting(false);

                    cols.Add("BankName")
                        .WithHeaderText("Bank Name")
                        .WithValueExpression(p => p.Jct_Dak_Register_Cheque.BankName);

                    cols.Add("Payable_At")
                        .WithHeaderText("Payable At")
                        .WithValueExpression(p => p.Jct_Dak_Register_Cheque.Payable_At);

                    cols.Add("Amount")
                        .WithHeaderText("Amount")
                        .WithValueExpression(p => p.Jct_Dak_Register_Cheque.Amount);
                   
                   
                  

                    //cols.Add("Remarks").WithHeaderText("Remarks")
                    //    .WithVisibility(true, false)
                    //    .WithSorting(false)
                    //    .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Remarks);

                    //cols.Add("Dak Type").WithHeaderText("Dak Type")
                    //   .WithVisibility(true, false)
                    //   .WithSorting(false)
                    //  .WithValueExpression(p => p.Jct_Dak_Register.Dak_Type);

                    //cols.Add("DakService_Type").WithHeaderText("Dak Service")
                    //    .WithVisibility(true, false)
                    //    .WithSorting(false)
                    //   .WithValueExpression(p => p.Jct_Dak_Register.DakService_Type);

                    cols.Add("ConcernedPersonals").WithHeaderText("ConcernedPersonals")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.ConcernedPersonals);
                    cols.Add("Dept_Code").WithHeaderText("Department")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.Department);
                    cols.Add("Received_Status")
                        .WithVisibility(visible: true, allowChangeVisibility: false)
                        .WithHeaderText("Status")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "<label class='label label-success'>Authorized</label>" : "<lable class='label label-danger'>Pending</label>")
                        .WithPlainTextValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "Authorized" : "Pending");
                    cols.Add("Created_On")
                        .WithVisibility(true, false)
                        .WithHeaderText("Entry Date")
                        .WithValueExpression(p => p.Jct_Dak_Register.Created_On.HasValue ? p.Jct_Dak_Register.Created_On.Value.ToShortDateString() : "")
                        .WithSorting(false);


                    //cols.Add("Replydate")
                    //    .WithHeaderText("Reply Date")
                    //    .WithVisibility(true, false)
                    //    .WithSorting(false)
                    //    .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.HasValue ? p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.Value.ToShortDateString() : "");
                })
                .WithAdditionalQueryOptionNames("startDate","endDate")
                .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                //.WithAdditionalSetting("", false)
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    
                DateTime startDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("startDate")).Date;
                    DateTime endDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("endDate")).Date;


                    var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                    using (var db = new DRSEntities())
                    {
                



                        var qry = (from p in db.Jct_Dak_Party_Detail_Address
                                   join c in db.Jct_Dak_Register on p.Party_Code equals c.Party_Code
                                   where p.Party_Code == c.Party_Code && p.address_id == c.PartyAddress_id


                                   join w in db.Jct_Dak_Party_Header on c.Party_Code equals w.Party_Code
                                   where w.Party_Code == c.Party_Code && w.Party_Type == c.Party_Type
                                   select new
                                   {
                                       p.Party_Code,
                                       w.Party_Name,
                                       p.Party_Type,
                                       p.City,
                                       p.address_id

                                   }).Distinct();


                        if (options.GetAdditionalQueryOptionString("startDate") == null && options.GetAdditionalQueryOptionString("endDate") == null)
                        {
                          var  query = (from m in db.Jct_Dak_Register

                                         join q in qry on m.Party_Code equals q.Party_Code
                                         where m.Party_Type == q.Party_Type
                                             || m.Party_Type == null || q.Party_Type == null

                                         join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No
                                         join d in db.Jct_Dak_Register_Cheque on m.Inward_No equals d.Inward_No into recive
                                         from d in recive
                                         where (m.Dak_Code =="CHQ")
                                         orderby m.Reference_Date descending
                                         select new DakManSys.ViewModel.GridViewModel
                                         {
                                             partyName = q.Party_Name,
                                             partyCity = q.City,
                                             Jct_Dak_Register = m,
                                             Jct_Dak_Register_Recieved = b,
                                             Jct_Dak_Register_Cheque = d
                                         }).AsQueryable();
                          result.TotalRecords = query.Count();
                          var globalSearch = options.GetAdditionalQueryOptionString("search");
                          if (!String.IsNullOrWhiteSpace(globalSearch))
                          {
                              query = query.Where(p => p.partyName.Contains(globalSearch));
                          }
                          if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                          {
                              switch (options.SortColumnName.ToLower())
                              {
                                  case "inward_no":
                                      query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                      break;
                                  case "party_name":
                                      query = query.OrderBy(p => p.partyName);
                                      break;
                                  case "received_status":
                                      query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                      break;
                              }
                          }
                          if (options.GetLimitOffset().HasValue)
                          {
                              query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                          }
                          result.Items = query.ToList();
                        }
                        else
                        { 
                       var query = (from m in db.Jct_Dak_Register

                                     join q in qry on m.Party_Code equals q.Party_Code
                                     where m.Party_Type == q.Party_Type
                                         || m.Party_Type == null || q.Party_Type == null

                                     join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No 
                                     join d in db.Jct_Dak_Register_Cheque on m.Inward_No equals d.Inward_No into recive
                                     from d in recive
                                     where (m.Dak_Code == "CHQ") && (m.Created_On >= startDate) && (m.Created_On <= endDate)                                  
                                     orderby m.Reference_Date descending
                                     select new DakManSys.ViewModel.GridViewModel
                                     {
                                         partyName = q.Party_Name,
                                         partyCity = q.City,
                                         Jct_Dak_Register = m,
                                         Jct_Dak_Register_Recieved = b,
                                         Jct_Dak_Register_Cheque=d
                                     }).AsQueryable();
                       result.TotalRecords = query.Count();
                       var globalSearch = options.GetAdditionalQueryOptionString("search");
                       if (!String.IsNullOrWhiteSpace(globalSearch))
                       {
                           query = query.Where(p => p.partyName.Contains(globalSearch));
                       }
                       if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                       {
                           switch (options.SortColumnName.ToLower())
                           {
                               case "inward_no":
                                   query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                   break;
                               case "party_name":
                                   query = query.OrderBy(p => p.partyName);
                                   break;
                               case "received_status":
                                   query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                   break;
                           }
                       }
                       if (options.GetLimitOffset().HasValue)
                       {
                           query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                       }
                       result.Items = query.ToList();

                        }
                       
                    }
                    return result;
                })
);
            //-----------------------------------------------------//




            //-----------------------------Parcel Report-------------//
            MVCGridDefinitionTable.Add("ParcelReport", new MVCGridBuilder<DakManSys.ViewModel.GridViewModel>(colDefauls)
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithAdditionalQueryOptionNames("search")
                .AddColumns(cols =>
                {
                    cols.Add("Inward_No")
                                            .WithHeaderText("InwardNo")
                                            .WithHtmlEncoding(false)
                                            .WithValueTemplate("<a href='' class='action'>{Model.Jct_Dak_Register.Inward_No}</a>")
                                            .WithPlainTextValueExpression(p => p.Jct_Dak_Register.Inward_No);
                    cols.Add("Party_Name").WithHeaderText("Party Name")
                       .WithVisibility(true, false)
                       .WithValueExpression(p => p.partyName);

                    cols.Add("City").WithHeaderText("City")
                  .WithVisibility(true, false)
                  .WithValueExpression(p => p.partyCity);

                    cols.Add("ReferenceNo").WithHeaderText("Refernce Number")
                       .WithVisibility(true, false)
                       .WithSorting(false)
                      .WithValueExpression(p => p.Jct_Dak_Register.ReferenceNo);
                    cols.Add("Reference_Date").WithHeaderText("Letter Date")
    .WithValueExpression(p => p.Jct_Dak_Register.Reference_Date.HasValue ? p.Jct_Dak_Register.Reference_Date.Value.ToShortDateString() : "")
    .WithVisibility(visible: true, allowChangeVisibility: false)
    .WithSorting(false);

                    cols.Add("DakService_Type").WithHeaderText("Dak Service")
                       .WithVisibility(true, false)
                       .WithSorting(false)
                      .WithValueExpression(p => p.Jct_Dak_Register.DakService_Type);


                    cols.Add("SUBJECT").WithHeaderText("Subject")
                       .WithVisibility(true, false)
                        .WithSorting(false)
                       .WithValueExpression(p => p.Jct_Dak_Register.SUBJECT);
                    //cols.Add("Remarks").WithHeaderText("Remarks")
                    //    .WithVisibility(true, false)
                    //    .WithSorting(false)
                    //    .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Remarks);

                    //cols.Add("Dak Type").WithHeaderText("Dak Type")
                    //   .WithVisibility(true, false)
                    //   .WithSorting(false)
                    //  .WithValueExpression(p => p.Jct_Dak_Register.Dak_Type);



                    cols.Add("ConcernedPersonals").WithHeaderText("ConcernedPersonals")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.ConcernedPersonals);
                    cols.Add("Dept_Code").WithHeaderText("Department")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithValueExpression(p => p.Jct_Dak_Register.Department);
                    cols.Add("Received_Status")
                        .WithVisibility(visible: true, allowChangeVisibility: false)
                        .WithHeaderText("Status")
                        .WithHtmlEncoding(false)
                        .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "<label class='label label-success'>Authorized</label>" : "<lable class='label label-danger'>Pending</label>")
                        .WithPlainTextValueExpression(p => p.Jct_Dak_Register_Recieved.Received_Status ? "Authorized" : "Pending");


                    cols.Add("Created_On")
                        .WithVisibility(true, false)
                        .WithHeaderText("Entry Date")
                        .WithValueExpression(p => p.Jct_Dak_Register.Created_On.HasValue ? p.Jct_Dak_Register.Created_On.Value.ToShortDateString() : "")
                        .WithSorting(false);

                    //cols.Add("Replydate")
                    //    .WithHeaderText("Reply Date")
                    //    .WithVisibility(true, false)
                    //    .WithSorting(false)
                    //    .WithValueExpression(p => p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.HasValue ? p.Jct_Dak_Register_Recieved.Reply_ReferenceDate.Value.ToShortDateString() : "");
                })
                .WithAdditionalQueryOptionNames("startDate", "endDate")
                .WithSorting(true, "Inward_No", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                //.WithAdditionalSetting("", false)
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;

                    DateTime startDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("startDate")).Date;
                    DateTime endDate = Convert.ToDateTime(options.GetAdditionalQueryOptionString("endDate")).Date;


                    var result = new QueryResult<DakManSys.ViewModel.GridViewModel>();
                    using (var db = new DRSEntities())
                    {




                        var qry = (from p in db.Jct_Dak_Party_Detail_Address
                                   join c in db.Jct_Dak_Register on p.Party_Code equals c.Party_Code
                                   where p.Party_Code == c.Party_Code && p.address_id == c.PartyAddress_id


                                   join w in db.Jct_Dak_Party_Header on c.Party_Code equals w.Party_Code
                                   where w.Party_Code == c.Party_Code && w.Party_Type == c.Party_Type
                                   select new
                                   {
                                       p.Party_Code,
                                       w.Party_Name,
                                       p.Party_Type,
                                       p.City,
                                       p.address_id

                                   }).Distinct();


                        if (options.GetAdditionalQueryOptionString("startDate") == null && options.GetAdditionalQueryOptionString("endDate") == null)
                        {
                            var query = (from m in db.Jct_Dak_Register

                                         join q in qry on m.Party_Code equals q.Party_Code
                                         where m.Party_Type == q.Party_Type
                                             || m.Party_Type == null || q.Party_Type == null

                                         join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No into recive
                                         from b in recive
                                         where (m.Dak_Code == "PAR")
                                         orderby m.Reference_Date descending
                                         select new DakManSys.ViewModel.GridViewModel
                                         {
                                             partyName = q.Party_Name,
                                             partyCity = q.City,
                                             Jct_Dak_Register = m,
                                             Jct_Dak_Register_Recieved = b
                                         }).AsQueryable();
                            result.TotalRecords = query.Count();
                            var globalSearch = options.GetAdditionalQueryOptionString("search");
                            if (!String.IsNullOrWhiteSpace(globalSearch))
                            {
                                query = query.Where(p => p.partyName.Contains(globalSearch));
                            }
                            if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                            {
                                switch (options.SortColumnName.ToLower())
                                {
                                    case "inward_no":
                                        query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                        break;
                                    case "party_name":
                                        query = query.OrderBy(p => p.partyName);
                                        break;
                                    case "received_status":
                                        query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                        break;
                                }
                            }
                            if (options.GetLimitOffset().HasValue)
                            {
                                query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                            }
                            result.Items = query.ToList();
                        }
                        else
                        {
                            var query = (from m in db.Jct_Dak_Register

                                         join q in qry on m.Party_Code equals q.Party_Code
                                         where m.Party_Type == q.Party_Type
                                             || m.Party_Type == null || q.Party_Type == null

                                         join b in db.Jct_Dak_Register_Recieved on m.Inward_No equals b.Inward_No into recive
                                         from b in recive
                                         where (m.Dak_Code == "PAR") && (m.Created_On >= startDate) && (m.Created_On <= endDate)
                                         orderby m.Reference_Date descending
                                         select new DakManSys.ViewModel.GridViewModel
                                         {
                                             partyName = q.Party_Name,
                                             partyCity = q.City,
                                             Jct_Dak_Register = m,
                                             Jct_Dak_Register_Recieved = b

                                         }).AsQueryable();
                            result.TotalRecords = query.Count();
                            var globalSearch = options.GetAdditionalQueryOptionString("search");
                            if (!String.IsNullOrWhiteSpace(globalSearch))
                            {
                                query = query.Where(p => p.partyName.Contains(globalSearch));
                            }
                            if (!String.IsNullOrWhiteSpace(options.SortColumnName))
                            {
                                switch (options.SortColumnName.ToLower())
                                {
                                    case "inward_no":
                                        query = query.OrderByDescending(p => p.Jct_Dak_Register.TransNo);
                                        break;
                                    case "party_name":
                                        query = query.OrderBy(p => p.partyName);
                                        break;
                                    case "received_status":
                                        query = query.OrderBy(p => p.Jct_Dak_Register_Recieved.Received_Status);
                                        break;
                                }
                            }
                            if (options.GetLimitOffset().HasValue)
                            {
                                query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                            }
                            result.Items = query.ToList();

                        }

                    }
                    return result;
                })
);
            //-------------------------------------------------------//
        }

    }
}