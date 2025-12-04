namespace AutogestionSena.MAUI.Api.Dtos
{
    public class InstructorDto
    {
        public int Id { get; set; }
        public int Person { get; set; }
        public bool Active { get; set; }
        public string? ContractType { get; set; }
        public string? ContractStartDate { get; set; }
        public string? ContractEndDate { get; set; }
        public int KnowledgeArea { get; set; }
    }

    public class CreateInstructorDto
    {
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? PhoneNumber { get; set; }
        public object? TypeIdentification { get; set; } // Puede ser int o un objeto
        public string? NumberIdentification { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
        public string? ContractType { get; set; }
        public string? ContractStartDate { get; set; }
        public string? ContractEndDate { get; set; }
        public int KnowledgeArea { get; set; }
        public int? Center { get; set; }
        public int? Sede { get; set; }
        public int? Regional { get; set; }
        public bool IsFollowupInstructor { get; set; }
    }

    public class InstructorCustomListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public object? KnowledgeArea { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public int? AssignedLearners { get; set; }
        public int? MaxAssignedLearners { get; set; }
        public object? Program { get; set; }
    }

    public class InstructorBackendResponseDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public int PhoneNumber { get; set; }
        public int TypeIdentification { get; set; }
        public string? NumberIdentification { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
        public int ContractType { get; set; }
        public string? ContractStartDate { get; set; }
        public string? ContractEndDate { get; set; }
        public int KnowledgeArea { get; set; }
        public int Sede { get; set; }
        public bool Active { get; set; }
    }
}