# VideoTeca

VideoTeca é uma plataforma web desenvolvida para gerenciar e catalogar vídeos educacionais na UNITINS. Ela oferece funcionalidades para que os professores enviem vídeos para revisão, gerenciem dados dos vídeos, categorizem em áreas e subáreas, e mantenham uma biblioteca de vídeos estruturada.

## Principais Funcionalidades
- **Gerenciamento de Usuários**: Criar, editar e gerenciar papéis e permissões de usuários.
- **Envio e Revisão de Vídeos**: Permite que professores enviem vídeos, que podem ser revisados e aprovados.
- **Organização por Área e Subárea**: Vídeos podem ser categorizados em áreas e subáreas específicas.
- **Busca e Filtro Avançados**: Facilita a busca e filtragem de vídeos por título, área, subárea e status.

## Propósito
O objetivo principal do VideoTeca é fornecer um sistema estruturado para organizar e gerenciar conteúdo educacional, facilitando para os professores o envio, categorização e também para os alunos e professores, a localização de vídeos relevantes para suas necessidades educacionais.

## Imagens do sistema
Página inicial
![image](https://github.com/user-attachments/assets/52d93292-f4fa-48c3-af1b-f1b5fce38c8d)
Listagem de videos
![image](https://github.com/user-attachments/assets/b8ed7064-ca6c-43ac-a7c2-493282f45b8a)
Filtro de permissões por usuário logado e notificações toast
![image](https://github.com/user-attachments/assets/36cbd5b1-34b0-483c-96f6-dd8b55cdc919)
Filtros básicos de pesquisa por grupo/nome de usuário no módulo do coordenador do projeto
![image](https://github.com/user-attachments/assets/2ae2023a-3dda-4acf-b840-82cff1059d9d)

## Tecnologias Utilizadas

- **Backend**: C# (.NET Framework)
- **Frontend**: HTML, CSS, JavaScript
- **Banco de Dados**: Entity Framework e SQL Server para gerenciamento de dados
- **Framework de UI**: Razor Pages para conteúdo dinâmico

## TO DO
- [X] Implementar autenticação de usuários.
- [X] Adicionar paginação nas listagens de vídeos.
- [X] Melhorar a responsividade da interface.
- [ ] Implementar uma página separada para exibição dos videos.

## Instalação e Configuração
Para configurar o projeto localmente, siga estes passos:
1. Clone o repositório:
   ```bash
   git clone https://github.com/gutinha/VideoTeca.git
   
2. Abra o arquivo de solução VideoTeca.sln no Visual Studio.
3. Configure a conexão com o banco de dados no arquivo web.config.
4. Compile e execute o projeto.

## Contribuição
Sinta-se à vontade para enviar issues, fazer fork do repositório e enviar pull requests se desejar contribuir para o projeto.
