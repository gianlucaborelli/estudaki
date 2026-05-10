# Migração de Dados - EstudaKi

## Visão Geral

Este documento descreve o processo de migração do modelo de dados antigo para o novo modelo no EstudaKi.

## Mudanças no Modelo

### Modelo Antigo
- **PublicNotice**: Continha todas as informações do edital, incluindo cargo e fase
- **Question**: Relacionada diretamente ao PublicNotice via `PublicNoticeId`

### Novo Modelo
- **PublicNotice2**: Contém informações gerais do edital (ano, banca, contratante)
  - **Exams** (documento filho): Lista de exames/cargos dentro do edital
    - Cada Exam representa um cargo/fase específico
    - Armazenado como array dentro do documento PublicNotice2
- **Question2**: Contém a questão sem referência direta ao edital
- **ExamQuestion**: Collection separada que relaciona Exam com Question2
  - Permite que uma questão seja reutilizada em múltiplos exames
  - Armazena informações específicas da questão no contexto do exame (número, anulação)
  - Referencia o Exam pelo seu Id (que está dentro de PublicNotice2)

## Estrutura de Coleções MongoDB

- `public_notices2` - Novos editais (contém array de Exams como documentos filhos)
- `questions2` - Novas questões
- `exam_questions` - Relacionamento entre exames e questões

### Exemplo de Documento PublicNotice2 no MongoDB

```json
{
  "_id": "507f1f77bcf86cd799439011",
  "Number": "001/2024",
  "Year": 2024,
  "ExaminerOrganization": "FGV",
  "ContractingOrganization": "Prefeitura de São Paulo",
  "ExamCategory": "PublicServiceExam",
  "IsReviewed": true,
  "IsPublished": true,
  "CreatedAt": "2024-01-15T10:00:00Z",
  "Exams": [
    {
      "_id": "507f1f77bcf86cd799439012",
      "Phase": "Objetiva",
      "Position": "Analista de Sistemas",
      "Area": "Tecnologia",
      "EducationLevel": "Superior",
      "ExamBookletUrl": "https://...",
      "AnswerKeyUrl": "https://...",
      "AnswerKeyItems": []
    },
    {
      "_id": "507f1f77bcf86cd799439013",
      "Phase": "Objetiva",
      "Position": "Desenvolvedor",
      "Area": "Tecnologia",
      "EducationLevel": "Superior",
      "ExamBookletUrl": "https://...",
      "AnswerKeyUrl": "https://...",
      "AnswerKeyItems": []
    }
  ]
}
```

### Exemplo de Documento ExamQuestion no MongoDB

```json
{
  "_id": "507f1f77bcf86cd799439014",
  "ExamId": "507f1f77bcf86cd799439012",
  "QuestionId": "507f1f77bcf86cd799439015",
  "IsNullified": false,
  "QuestionNumber": 1
}
```

**Nota Importante**: O `ExamId` no documento ExamQuestion referencia o Id do Exam que está **dentro** do array Exams de um PublicNotice2.

## Como Executar a Migração

### Pré-requisitos
- Estar autenticado na aplicação (controller protegido com [Authorize])
- Ter acesso ao ambiente de produção/staging

### Endpoint de Migração

```http
POST /api/migration/migrate
```

Este endpoint irá:
1. Buscar todos os editais antigos (public_notices)
2. Para cada edital:
   - Criar um Exam (documento filho) baseado nas informações do edital antigo (Phase, Position)
   - Criar um novo PublicNotice2 contendo o Exam no array Exams
   - Buscar todas as questões relacionadas
   - Para cada questão:
     - Criar uma nova Question2
     - Criar um ExamQuestion (collection separada) vinculando o ExamId à QuestionId
   - Salvar o PublicNotice2 com todos os dados (incluindo os Exams como documentos filhos)

### Endpoint de Status

```http
GET /api/migration/status
```

Retorna:
```json
{
  "oldData": {
    "publicNotices": 100,
    "questions": 5000
  },
  "newData": {
    "publicNotices": 100,
    "questions": 5000
  }
}
```

## Resposta da Migração

```json
{
  "publicNoticesMigrated": 100,
  "examsCreated": 100,
  "questionsMigrated": 5000,
  "examQuestionsCreated": 5000,
  "errors": []
}
```

## Mapeamento de Campos

### PublicNotice → PublicNotice2
- `Id` → `Id` (mantém o mesmo)
- `Number` → `Number`
- `Year` → `Year`
- `ExamBoard` → `ExaminerOrganization`
- `ExamRequester` → `ContractingOrganization`
- `ExamCategory` → `ExamCategory`
- `IsReviewed` → `IsReviewed`
- `IsPublished` → `IsPublished`
- `CreatedAt` → `CreatedAt`

### PublicNotice → Exam
- `ExamPhase` → `Phase`
- `Position` → `Position`

### Question → Question2
- `Id` → `Id` (mantém o mesmo)
- `QuestionNumber` → `Number`
- `Type` → `Type`
- `MainArea` → `MainArea`
- `SubAreas` → `SubAreas`
- `QuestionSupports` → `QuestionSupports`
- `QuestionContents` → `QuestionContents`
- `Choices` → `Choices`
- `CreatedAt` → `CreatedAt`
- `IsPublished` → `IsPublished`

### Question → ExamQuestion
- `IsNullified` → `IsNullified`
- `QuestionNumber` → `QuestionNumber`

## Novos Repositórios

### Interfaces
- `IPublicNoticeRepository2` - Repositório para PublicNotice2 (que contém Exams como documentos filhos)
- `IQuestionRepository2` - Repositório para Question2
- `IExamQuestionRepository` - Repositório para ExamQuestion (relacionamento Exam-Question)

### Implementações
- `PublicNoticeRepository2` - Gerencia PublicNotice2 e seus Exams filhos
- `QuestionRepository2` - Gerencia Question2
- `ExamQuestionRepository` - Gerencia ExamQuestion

**Nota**: Não há repositório separado para Exam, pois ele é um documento filho dentro de PublicNotice2.

Todos os repositórios estão registrados no DI container em `ServiceCollectionExtensions.cs`.

## Observações Importantes

1. **IDs mantidos**: Os IDs das entidades PublicNotice e Question são mantidos para facilitar a transição
2. **Queries antigas**: As queries antigas continuam funcionando, permitindo uma migração gradual
3. **Rollback**: Em caso de problemas, as coleções antigas não são modificadas
4. **Logging**: Todo o processo de migração é registrado com logs detalhados
5. **Erros**: Erros individuais não interrompem a migração completa - são registrados e retornados

## Próximos Passos

Após a migração bem-sucedida:
1. Verificar os dados nas novas coleções
2. Atualizar as telas de exibição para usar os novos repositórios
3. Atualizar as queries para usar o novo modelo
4. Remover o código antigo quando não for mais necessário
