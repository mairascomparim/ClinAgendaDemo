# 🏥 ClinAgenda - Backend

Este projeto foi desenvolvido para o **BootCamp Curso Intensivo de Desenvolvimento Web FullStack com ASP.NET Core & Vue🚀**, uma parceria entre o **PECEGE** e o **DEVPIRA**.

Com aulas ministradas por Decio Stenico e Maira Scomparim, a proposta do projeto é construir uma aplicação completa para uso em uma clínica médica, com funcionalidades voltadas à gestão de profissionais, pacientes e agendamentos.

Este repositório contém **o backend da aplicação**, desenvolvido em **C# com ASP.NET Core**, seguindo os princípios da **Clean Architecture** para garantir uma estrutura escalável, de fácil manutenção e bem organizada.

🔗 **Frontend do projeto disponível em:** [*https://github.com/stenicodecio/bootcamp-clinagenda-frontend*]

---

## 🛠️ Tecnologias Utilizadas

- **C#**
- **ASP.NET Core**
- **Clean Architecture**
- **MySQL** 
- **Dapper** (para mapeamento objeto-relacional leve)
- **Swagger** (para documentação e testes da API)

---

## 🗂️ Funcionalidades Implementadas

Durante o desenvolvimento, implementamos as operações básicas de CRUD (Create, Read, Update, Delete) para as principais entidades do sistema:

- **Profissionais de Saúde**: Cadastro, consulta, atualização e remoção de profissionais que atendem na clínica.
- **Pacientes**: Gerenciamento completo dos pacientes, incluindo histórico e dados pessoais.
- **Agendamentos**: Controle de consultas e procedimentos agendados, permitindo marcação, alteração e cancelamento de horários.

Essas funcionalidades permitem uma gestão eficiente e integrada dos processos da clínica, facilitando o dia a dia dos usuários.

---

## 🚀 Como Executar o Projeto

1. **Clone este repositório**:
   ```bash
   git clone https://github.com/mairascomparim/ClinAgendaDemo.git
   ```

2. **Navegue até o diretório do projeto**:
   ```bash
   cd ClinAgendaDemo
   ```

3. **Configure a string de conexão** no arquivo `appsettings.json` com as credenciais do seu banco de dados MySQL.

4. **Execute os comandos para criar e popular seu banco de dados: [https://respected-day-c3a.notion.site/Cria-o-Banco-de-dados-ClinAgenda-180c54029eb58074b4c5d40373e8785e]

5. **Inicie a aplicação**:
   ```bash
   dotnet run
   ```

6. **Acesse a documentação da API** no Swagger através do navegador:
   ```
   http://localhost:5000/swagger
   ```

---
