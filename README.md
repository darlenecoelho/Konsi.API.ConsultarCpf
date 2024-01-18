# Sistema para consultar benefícios
Konsi API é um sistema backend desenvolvido para consultar benefícios associados a um CPF. Ele integra diferentes serviços externos e bases de dados, incluindo Elasticsearch e Redis, para fornecer informações rápidas e precisas. Utiliza Vue.js para a interface do usuário e se comunica com uma API Elasticsearch para recuperar os dados. Inclui funcionalidades como validação de entrada de CPF e exibição de resultados em um modal.

## Tecnologias Backend: 

- ASP.NET Core para a construção da API.
- Elasticsearch para indexação e busca de dados.
- Redis para cacheamento de dados.
- RabbitMQ para gerenciamento de filas de mensagens.
- Docker e Docker Compose para containerização e orquestração.

## Tecnologias UI
-Vue.js.
-Axios para requisições HTTP.
- Vue Toast Notification para exibição de mensagens.
-Tailwind CSS para estilização.

## Recursos UI
- Exibição de informações detalhadas (como benefícios associados ao CPF) em um modal.
- Mensagens de erro para CPFs não encontrados.
- Interface responsiva.

## Executando a aplicação usando o Docker

Após executar o comando no terminal `docker-compose up --build`, basta abrir a url no navegador: `http://localhost:8000/swagger/`.

## Acessando a Interface do RabbitMQ
Após subir os serviços com Docker Compose, a interface de gerenciamento do RabbitMQ estará disponível em http://localhost:15672.

Usuário padrão: guest
Senha padrão: guest

Essa interface permite monitorar filas, mensagens, conexões, entre outras funcionalidades.

## Uso da API:

- Consulta de Benefícios
Endpoint: GET /konsi/consultar-beneficios/{cpf}.
Descrição: Este endpoint permite consultar os benefícios associados a um CPF específico. Ao receber uma requisição, a API primeiro publica o CPF na fila de mensagens e, em seguida, busca os dados de benefícios. Se os dados estiverem disponíveis no cache (Redis), eles são retornados imediatamente. Caso contrário, a API os busca através de um serviço externo, armazena os resultados no cache para consultas futuras e retorna esses dados ao usuário.
Parâmetros:
cpf: O CPF do indivíduo para o qual os benefícios devem ser consultados.

- Consulta no Elasticsearch
Endpoint: GET /konsi/elasticsearch/{cpf}
Descrição: Este endpoint realiza uma consulta para obter dados de benefícios associados a um CPF diretamente do Elasticsearch. Essa consulta é útil para verificar dados que foram indexados previamente no Elasticsearch após serem obtidos de uma API externa ou do cache. Esse método oferece uma busca rápida e eficiente.
Parâmetros:
cpf: O CPF do indivíduo para o qual os benefícios devem ser consultados no Elasticsearch.

