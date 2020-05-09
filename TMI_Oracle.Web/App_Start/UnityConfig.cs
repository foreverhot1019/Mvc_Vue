using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Ef6;
using Repository.Pattern.DataContext;
using TMI.Web.Models;
using Repository.Pattern.Repositories;
using TMI.Web.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Microsoft.Owin.Security;
using System.Web;

namespace TMI.Web.App_Start
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

            container.RegisterType<IRepositoryAsync<PARA_CURR>, Repository<PARA_CURR>>();
            container.RegisterType<IPARA_CURRService, PARA_CURRService>();

            container.RegisterType<IRepositoryAsync<PARA_Country>, Repository<PARA_Country>>();
            container.RegisterType<IPARA_CountryService, PARA_CountryService>();

            container.RegisterType<IRepositoryAsync<DailyRate>, Repository<DailyRate>>();
            container.RegisterType<IDailyRateService, DailyRateService>();

            container.RegisterType<IRepositoryAsync<Rate>, Repository<Rate>>();
            container.RegisterType<IRateService, RateService>();

            container.RegisterType<IRepositoryAsync<Customer>, Repository<Customer>>();
            container.RegisterType<ICustomerService, CustomerService>();

            container.RegisterType<IRepositoryAsync<AdvisoryOrder>, Repository<AdvisoryOrder>>();
            container.RegisterType<IAdvisoryOrderService, AdvisoryOrderService>();

            container.RegisterType<IRepositoryAsync<Order>, Repository<Order>>();
            container.RegisterType<IOrderService, OrderService>();

            container.RegisterType<IRepositoryAsync<ActualMoney>, Repository<ActualMoney>>();
            container.RegisterType<IActualMoneyService, ActualMoneyService>();

            container.RegisterType<IRepositoryAsync<FinanceMoney>, Repository<FinanceMoney>>();
            container.RegisterType<IFinanceMoneyService, FinanceMoneyService>();

            container.RegisterType<IRepositoryAsync<CostMoney>, Repository<CostMoney>>();
            container.RegisterType<ICostMoneyService, CostMoneyService>();

            container.RegisterType<IRepositoryAsync<Supplier>, Repository<Supplier>>();
            container.RegisterType<ISupplierService, SupplierService>();

            container.RegisterType<IRepositoryAsync<AirTicketOrder>, Repository<AirTicketOrder>>();
            container.RegisterType<IAirTicketOrderService, AirTicketOrderService>();

            container.RegisterType<IRepositoryAsync<PlanePerson>, Repository<PlanePerson>>();
            container.RegisterType<IPlanePersonService, PlanePersonService>();

            container.RegisterType<IRepositoryAsync<AirLine>, Repository<AirLine>>();
            container.RegisterType<IAirLineService, AirLineService>();

            #endregion

            container.RegisterType<IRepositoryAsync<TestMVC_CRUD>, Repository<TestMVC_CRUD>>();
            container.RegisterType<ITestMVC_CRUDService, TestMVC_CRUDService>();
        }
    }
}
