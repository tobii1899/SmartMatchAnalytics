const fs = require('fs')

const cfg = {
  apiBaseUrl: process.env.services__webapi__https__0 || process.env.services__webapi__http__0,
  port: process.env.PORT || 4200,
}

fs.writeFileSync(
  'src/environments/environment.development.ts',
  `export const environment = { apiBaseUrl: '${cfg.apiBaseUrl}' }\n`
)

// Output the port for use in the start script
console.log(cfg.port)
