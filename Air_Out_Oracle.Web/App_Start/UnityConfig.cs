using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Ef6;
using Repository.Pattern.DataContext;
using AirOut.Web.Models;
using Repository.Pattern.Repositories;
using AirOut.Web.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Microsoft.Owin.Security;
using System.Web;

namespace AirOut.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// 单例模式依赖注入
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container

        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        public static Lazy<IUnityContainer> Instance
        {
            get { return container; }
        }

        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            //container.RegisterType<IUnitOfWorkAsync, UnitOfWork>(new HierarchicalLifetimeManager());
            //container.RegisterType<IDataContextAsync, StoreContext>(new HierarchicalLifetimeManager());

            #region 注册权限 和 DbContext

            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());

            container.RegisterType<IRoleStore<ApplicationRole, string>, RoleStore<ApplicationRole>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthenticationManager>(new InjectionFactory(o => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerRequestLifetimeManager());
            container.RegisterType<IDataContextAsync, WebdbContext>(new PerRequestLifetimeManager());

            #endregion

            #region 菜单和权限

            container.RegisterType<IRepositoryAsync<MenuAction>, Repository<MenuAction>>();
            container.RegisterType<IMenuActionService, MenuActionService>();

            container.RegisterType<IRepositoryAsync<MenuItem>, Repository<MenuItem>>();
            container.RegisterType<IMenuItemService, MenuItemService>();

            container.RegisterType<IRepositoryAsync<RoleMenu>, Repository<RoleMenu>>();
            container.RegisterType<IRoleMenuService, RoleMenuService>();

            #endregion

            container.RegisterType<IRepositoryAsync<Company>, Repository<Company>>();
            container.RegisterType<ICompanyService, CompanyService>();

            container.RegisterType<IRepositoryAsync<BaseCode>, Repository<BaseCode>>();
            container.RegisterType<IBaseCodeService, BaseCodeService>();

            container.RegisterType<IRepositoryAsync<CodeItem>, Repository<CodeItem>>();
            container.RegisterType<ICodeItemService, CodeItemService>();

            container.RegisterType<IRepositoryAsync<DataTableImportMapping>, Repository<DataTableImportMapping>>();
            container.RegisterType<IDataTableImportMappingService, DataTableImportMappingService>();

            container.RegisterType<IRepositoryAsync<Notification>, Repository<Notification>>();
            container.RegisterType<INotificationService, NotificationService>();

            container.RegisterType<IRepositoryAsync<Message>, Repository<Message>>();
            container.RegisterType<IMessageService, MessageService>();

            container.RegisterType<IRepositoryAsync<ChangeOrderHistory>, Repository<ChangeOrderHistory>>();
            container.RegisterType<IChangeOrderHistoryService, ChangeOrderHistoryService>();

            container.RegisterType<IRepositoryAsync<MailReceiver>, Repository<MailReceiver>>();
            container.RegisterType<IMailReceiverService, MailReceiverService>();

            #region 操作点

            container.RegisterType<IRepositoryAsync<OperatePoint>, Repository<OperatePoint>>();
            container.RegisterType<IOperatePointService, OperatePointService>();

            container.RegisterType<IRepositoryAsync<OperatePointList>, Repository<OperatePointList>>();
            container.RegisterType<IOperatePointListService, OperatePointListService>();
            //用户和操作点关联
            container.RegisterType<IRepositoryAsync<UserOperatePointLink>, Repository<UserOperatePointLink>>();
            container.RegisterType<IUserOperatePointLinkService, UserOperatePointLinkService>();

            #endregion

            #region 基础资料

            container.RegisterType<IRepositoryAsync<BD_DEFDOC>, Repository<BD_DEFDOC>>();
            container.RegisterType<IBD_DEFDOCService, BD_DEFDOCService>();

            container.RegisterType<IRepositoryAsync<BD_DEFDOC_LIST>, Repository<BD_DEFDOC_LIST>>();
            container.RegisterType<IBD_DEFDOC_LISTService, BD_DEFDOC_LISTService>();

            container.RegisterType<IRepositoryAsync<PARA_Area>, Repository<PARA_Area>>();
            container.RegisterType<IPARA_AreaService, PARA_AreaService>();

            container.RegisterType<IRepositoryAsync<PARA_AirLine>, Repository<PARA_AirLine>>();
            container.RegisterType<IPARA_AirLineService, PARA_AirLineService>();

            container.RegisterType<IRepositoryAsync<PARA_CURR>, Repository<PARA_CURR>>();
            container.RegisterType<IPARA_CURRService, PARA_CURRService>();

            container.RegisterType<IRepositoryAsync<PARA_Customs>, Repository<PARA_Customs>>();
            container.RegisterType<IPARA_CustomsService, PARA_CustomsService>();

            container.RegisterType<IRepositoryAsync<PARA_Country>, Repository<PARA_Country>>();
            container.RegisterType<IPARA_CountryService, PARA_CountryService>();

            container.RegisterType<IRepositoryAsync<DailyRate>, Repository<DailyRate>>();
            container.RegisterType<IDailyRateService, DailyRateService>();

            container.RegisterType<IRepositoryAsync<PARA_AirPort>, Repository<PARA_AirPort>>();
            container.RegisterType<IPARA_AirPortService, PARA_AirPortService>();

            container.RegisterType<IRepositoryAsync<PARA_AirPort>, Repository<PARA_AirPort>>();
            container.RegisterType<IPARA_AirPortService, PARA_AirPortService>();

            container.RegisterType<IRepositoryAsync<Rate>, Repository<Rate>>();
            container.RegisterType<IRateService, RateService>();

            container.RegisterType<IRepositoryAsync<FeeType>, Repository<FeeType>>();
            container.RegisterType<IFeeTypeService, FeeTypeService>();

            container.RegisterType<IRepositoryAsync<FeeUnit>, Repository<FeeUnit>>();
            container.RegisterType<IFeeUnitService, FeeUnitService>();

            container.RegisterType<IRepositoryAsync<DealArticle>, Repository<DealArticle>>();
            container.RegisterType<IDealArticleService, DealArticleService>();

            container.RegisterType<IRepositoryAsync<PARA_Package>, Repository<PARA_Package>>();
            container.RegisterType<IPARA_PackageService, PARA_PackageService>();
            #endregion

            #region 客商管理

            container.RegisterType<IRepositoryAsync<CusBusInfo>, Repository<CusBusInfo>>();
            container.RegisterType<ICusBusInfoService, CusBusInfoService>();

            container.RegisterType<IRepositoryAsync<CostMoney>, Repository<CostMoney>>();
            container.RegisterType<ICostMoneyService, CostMoneyService>();

            container.RegisterType<IRepositoryAsync<QuotedPrice>, Repository<QuotedPrice>>();
            container.RegisterType<IQuotedPriceService, QuotedPriceService>();

            container.RegisterType<IRepositoryAsync<CustomerQuotedPrice>, Repository<CustomerQuotedPrice>>();
            container.RegisterType<ICustomerQuotedPriceService, CustomerQuotedPriceService>();

            container.RegisterType<IRepositoryAsync<CusQuotedPriceDtl>, Repository<CusQuotedPriceDtl>>();
            container.RegisterType<ICusQuotedPriceDtlService, CusQuotedPriceDtlService>();

            container.RegisterType<IRepositoryAsync<Contacts>, Repository<Contacts>>();
            container.RegisterType<IContactsService, ContactsService>();

            #endregion

            #region 承揽接单

            container.RegisterType<IRepositoryAsync<OPS_EntrustmentInfor>, Repository<OPS_EntrustmentInfor>>();
            container.RegisterType<IOPS_EntrustmentInforService, OPS_EntrustmentInforService>();

            container.RegisterType<IRepositoryAsync<OPS_M_Order>, Repository<OPS_M_Order>>();
            container.RegisterType<IOPS_M_OrderService, OPS_M_OrderService>();

            container.RegisterType<IRepositoryAsync<OPS_H_Order>, Repository<OPS_H_Order>>();
            container.RegisterType<IOPS_H_OrderService, OPS_H_OrderService>();

            container.RegisterType<IRepositoryAsync<CustomsInspection>, Repository<CustomsInspection>>();
            container.RegisterType<ICustomsInspectionService, CustomsInspectionService>();

            container.RegisterType<IRepositoryAsync<DocumentManagement>, Repository<DocumentManagement>>();
            container.RegisterType<IDocumentManagementService,DocumentManagementService>();

            #endregion

            #region 仓库接单

            container.RegisterType<IRepositoryAsync<Warehouse_receipt>, Repository<Warehouse_receipt>>();
            container.RegisterType<IWarehouse_receiptService, Warehouse_receiptService>();

            container.RegisterType<IRepositoryAsync<Warehouse_Cargo_Size>, Repository<Warehouse_Cargo_Size>>();
            container.RegisterType<IWarehouse_Cargo_SizeService, Warehouse_Cargo_SizeService>();
            #endregion

            #region 图片

            container.RegisterType<IRepositoryAsync<Picture>, Repository<Picture>>();
            container.RegisterType<IPictureService, PictureService>();

            #endregion

            #region 结算

            container.RegisterType<IRepositoryAsync<Bms_Bill_Ap>, Repository<Bms_Bill_Ap>>();
            container.RegisterType<IBms_Bill_ApService, Bms_Bill_ApService>();

            container.RegisterType<IRepositoryAsync<Bms_Bill_Ap_Dtl>, Repository<Bms_Bill_Ap_Dtl>>();
            container.RegisterType<IBms_Bill_Ap_DtlService, Bms_Bill_Ap_DtlService>();

            container.RegisterType<IRepositoryAsync<Bms_Bill_Ar>, Repository<Bms_Bill_Ar>>();
            container.RegisterType<IBms_Bill_ArService, Bms_Bill_ArService>();

            container.RegisterType<IRepositoryAsync<Bms_Bill_Ar_Dtl>, Repository<Bms_Bill_Ar_Dtl>>();
            container.RegisterType<IBms_Bill_Ar_DtlService, Bms_Bill_Ar_DtlService>();

            #endregion

            container.RegisterType<IRepositoryAsync<CoPoKind>, Repository<CoPoKind>>();
            container.RegisterType<ICoPoKindService, CoPoKindService>();

            container.RegisterType<IRepositoryAsync<TradeType>, Repository<TradeType>>();
            container.RegisterType<ITradeTypeService, TradeTypeService>();

            container.RegisterType<IRepositoryAsync<PortalEntryIDLink>, Repository<PortalEntryIDLink>>();
            container.RegisterType<IPortalEntryIDLinkService, PortalEntryIDLinkService>();

            container.RegisterType<IRepositoryAsync<Eai_Group>, Repository<Eai_Group>>();
            container.RegisterType<IEai_GroupService, Eai_GroupService>();

            //账务操作，应收应付 数据拼接
            container.RegisterType<IFinanceService, FinanceService>();

            //销售人
            container.RegisterType<IRepositoryAsync<Saller>, Repository<Saller>>();
            container.RegisterType<ISallerService, SallerService>();

            container.RegisterType<IRepositoryAsync<TestMVC_CRUD>, Repository<TestMVC_CRUD>>();
            container.RegisterType<ITestMVC_CRUDService, TestMVC_CRUDService>();
        }
    }
}
