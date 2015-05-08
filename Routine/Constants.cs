
namespace Routine
{
	internal class Constants
	{
		public const int DEFAULT_MAX_FETCH_DEPTH = 10;
		public const int FIRST_DEPTH = 1;

		public const string INITIALIZER_ID = "__routine_initializer";
		public const string DOMAIN_TYPES_CACHE_KEY = "__routine_domain_types";
		public const string APPLICATION_MODEL_CACHE_KEY = "__routine_application_model";

		public const string VOID_MODEL_ID = "__routine_void";
		public const string NULL_MODEL_ID = "__routine_null";

		public const string DEFAULT_VIRTUAL_MARK = "__routine_virtual";

		public const string PROPERTY_OPERATION_DEFAULT_PREFIX = "Get";

		public const int DEFAULT_MAX_RESULT_LENGTH = 2 * 1024 * 1024;

		public const string SERVICE_ROUTE_NAME = "__routine_service_route";
		public const string SERVICE_RESPONSE_HEADER_PREFIX = "X-Routine-";
		
		public const string MVC_PERFORM_ROUTE_NAME = "__routine_mvc_perform_route";
		public const string MVC_PERFORM_AS_ROUTE_NAME = "__routine_mvc_perform_as_route";
		public const string MVC_GET_ROUTE_NAME = "__routine_mvc_get_route";
		public const string MVC_GET_AS_ROUTE_NAME = "__routine_mvc_get_as_route";

	}
}
