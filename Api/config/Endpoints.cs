using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutogestionSena.MAUI.Api
{
    public static class Endpoints
    {
        // ============================================
        // CONFIGURACIÓN DEL SERVIDOR
        // ============================================
        // DESARROLLO LOCAL: Descomentar estas líneas
        // private const string BASE_IP = "192.168.1.12";
        // private const string BASE_PORT = "8000";
        
        // PRODUCCIÓN: Servidor desplegado
        private const string BASE_IP = "167.114.98.199";
        private const string BASE_PORT = "8000";  // Puerto del backend Django
        // ============================================
        
        // Configuración de URL base según la plataforma
        // Para emulador Android: usa 10.0.2.2 en lugar de localhost/127.0.0.1
        // Para dispositivo físico Android: usa la IP de tu red local
        // Para iOS: usa la IP de tu red local
        // Para Windows: usa localhost o la IP de tu red local
        private static string GetBaseUrl()
        {
#if ANDROID
            // Para Android usamos la IP directa
            return $"http://{BASE_IP}/api/";
#elif IOS
            // Para iOS usamos la IP de la red local
            return $"http://{BASE_IP}/api/";
#else
            // Para Windows y otras plataformas
            return $"http://{BASE_IP}/api/";
#endif
        }

        public static string API_BASE_URL => GetBaseUrl();

        public static class Person
        {
            public static string RegisterApprentice => $"{API_BASE_URL}security/persons/register-apprentice/";
            public static string IdPerson(int id) => $"{API_BASE_URL}security/persons/{id}/";
        }

        public static class DocumentType
        {
            public static string GetAll => $"{API_BASE_URL}security/document-types/";
        }

        public static class User
        {
            public static string ValidateLogin => $"{API_BASE_URL}security/users/validate-institutional-login/";
            public static string ValidateSecondFactor => $"{API_BASE_URL}security/users/validate-2fa-code/";
            public static string GetUser => $"{API_BASE_URL}security/users/";
            public static string RequestPasswordReset => $"{API_BASE_URL}security/users/request-password-reset/";
            public static string ResetPassword => $"{API_BASE_URL}security/users/reset-password/";
            public static string GetUserId(int id) => $"{API_BASE_URL}security/users/{id}/";
            public static string DeleteUser(int id) => $"{API_BASE_URL}security/users/{id}/soft-delete/";
            public static string Filter => $"{API_BASE_URL}security/users/filter/";
        }

        public static class Menu
        {
            public static string GetMenuItems(int id) => $"{API_BASE_URL}security/rol-form-permissions/{id}/get-menu/";
        }

        public static class Rol
        {
            public static string GetRoles => $"{API_BASE_URL}security/roles/";
            public static string GetRolUser => $"{API_BASE_URL}security/roles/roles-with-user-count/";
            public static string PostRolPermissions => $"{API_BASE_URL}security/rol-form-permissions/create-role-with-permissions/";
            public static string GetRolPermissions(int id) => $"{API_BASE_URL}security/rol-form-permissions/{id}/get-role-with-permissions/";
            public static string PutRolFormPerms(int id) => $"{API_BASE_URL}security/rol-form-permissions/{id}/update-role-with-permissions/";
            public static string GetRolesFormsPerms => $"{API_BASE_URL}security/rol-form-permissions/permission-matrix/";
            public static string DeleteRolUsers(int id) => $"{API_BASE_URL}security/roles/{id}/logical-delete-with-users/";
            public static string FilterRol => $"{API_BASE_URL}security/roles/filter/";
        }

        public static class Form
        {
            public static string GetForm => $"{API_BASE_URL}security/forms/";
            public static string DeleteForm(int id) => $"{API_BASE_URL}security/forms/{id}/";
            public static string Post => $"{API_BASE_URL}security/forms/";
        }

        public static class Module
        {
            public static string GetModule => $"{API_BASE_URL}security/modules/";
            public static string DeleteModule(int id) => $"{API_BASE_URL}security/modules/{id}/soft-delete/";
            public static string Post => $"{API_BASE_URL}security/form-modules/create-module-with-forms/";
            public static string GetModuleForms(int id) => $"{API_BASE_URL}security/form-modules/{id}/get-module-with-forms/";
            public static string PutModuleForms(int id) => $"{API_BASE_URL}security/form-modules/{id}/update-module-with-forms/";
            public static string FilterModules => $"{API_BASE_URL}security/modules/filter/";
        }

        public static class Apprentice
        {
            public static string AllApprentices => $"{API_BASE_URL}general/aprendices/Create-Aprendiz/create/";
            public static string GetAllApprentices => $"{API_BASE_URL}general/aprendices/";
            public static string PutIdApprentice(int id) => $"{API_BASE_URL}general/aprendices/{id}/Create-Aprendiz/update/";
        }

        public static class Instructor
        {
            public static string AllInstructores => "general/instructors/Create-Instructor/create/";
            public static string GetAllInstructores => "general/instructors/";
            public static string PutIdInstructor(int id) => $"general/instructors/{id}/Create-Instructor/update/";
            public static string GetCustomList => "general/instructors/custom-list/";
            public static string PatchLimit(int id) => $"general/instructors/{id}/update-learners/";
            public static string FilterInstructores => "general/instructors/filter/";
    
            public static string GetInstructor(int instructorId) => $"general/instructors/{instructorId}/";
        }

        public static class Regional
        {
            public static string AllRegionals => $"{API_BASE_URL}general/regionals/";
            public static string IdRegionals(int id) => $"{API_BASE_URL}general/regionals/{id}/";
            public static string SoftDeleteRegionals(int id) => $"{API_BASE_URL}general/regionals/{id}/soft-delete/";
        }

        public static class Center
        {
            public static string AllCenters => $"{API_BASE_URL}general/centers/";
            public static string IdCenters(int id) => $"{API_BASE_URL}general/centers/{id}/";
            public static string SoftDeleteCenters(int id) => $"{API_BASE_URL}general/centers/{id}/soft-delete/";
        }

        public static class Sede
        {
            public static string AllSedes => $"{API_BASE_URL}general/sedes/";
            public static string IdSedes(int id) => $"{API_BASE_URL}general/sedes/{id}/";
            public static string SoftDeleteSedes(int id) => $"{API_BASE_URL}general/sedes/{id}/soft-delete/";
        }

        public static class Program
        {
            public static string AllPrograms => $"{API_BASE_URL}general/programs/";
            public static string GetProgramFicha(int id) => $"{API_BASE_URL}general/programs/{id}/fichas/";
            public static string IdProgram(int id) => $"{API_BASE_URL}general/programs/{id}/";
            public static string DeleteIdProgram(int id) => $"{API_BASE_URL}general/programs/{id}/disable-with-fichas/";
        }

        public static class KnowledgeArea
        {
            public static string AllKnowledgeAreas => $"{API_BASE_URL}general/knowledge-areas/";
            public static string IdKnowledgeArea(int id) => $"{API_BASE_URL}general/knowledge-areas/{id}/";
            public static string DeleteIdKnowledgeArea(int id) => $"{API_BASE_URL}general/knowledge-areas/{id}/soft-delete/";
        }

        public static class Ficha
        {
            public static string AllFichas => $"{API_BASE_URL}general/fichas/";
            public static string IdFicha(int id) => $"{API_BASE_URL}general/fichas/{id}/";
            public static string DeleteIdFicha(int id) => $"{API_BASE_URL}general/fichas/{id}/soft-delete/";
        }

        public static class Permission
        {
            public static string GetPermissions => $"{API_BASE_URL}security/permissions/";
        }

        public static class Assignment
        {
            public static string GetFormRequestList => "assign/request_asignation/form-request-list/";
            public static string GetApprenticeDashboard(int apprenticeId) 
            {
                var endpoint = $"assign/request_asignation/aprendiz-dashboard/?aprendiz_id={apprenticeId}";
                return endpoint;
            }
            public static string GetEnterprise(int enterpriseId) => $"assign/enterprise/{enterpriseId}/";
            public static string GetModalityProductiveStage => "assign/modality_productive_stage/";
        }

        public static class ApprenticeSimple
        {
            public static string GetAllApprenticesSimple => $"{API_BASE_URL}general/aprendices/";
        }

        public static class Notification
        {
            public static string GetNotifications => $"{API_BASE_URL}general/notifications/";
            public static string DeleteById => $"{API_BASE_URL}general/notifications/delete-by-id/";
            public static string DeleteByUser => $"{API_BASE_URL}general/notifications/delete-by-user/";
            public static string GetById(int id) => $"{API_BASE_URL}general/notifications/{id}/";
        }
    }
}
