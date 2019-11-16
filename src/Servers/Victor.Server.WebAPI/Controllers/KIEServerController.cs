using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Victor.CUI.RHDM.KIE.Client;
using Victor.CUI.RHDM.KIE.Api;
using Victor.CUI.RHDM.KIE.Model;

namespace Victor.Server.WebAPI
{
    [Produces("application/json")]
    [Route("api/kie_server")]
    [ApiController]
    public class KIEServerController : ControllerBase
    {
        protected ILogger<KIEServerController> Logger { get; }

        protected KIEServerAndKIEContainersApi CApi { get; }
        
        protected KIESessionAssetsApi SApi { get; }
        public KIEServerController(ILogger<KIEServerController> logger, KIEServerAndKIEContainersApi cApi, KIESessionAssetsApi sApi)
        {
            Logger = logger;
            CApi = cApi;
            SApi = sApi;
        }

        // GET api/fruits
        [HttpGet]
        public async Task<ActionResult<List<HealthCheck>>> Get() => await CApi.HealthcheckAsync(true);
        
        /*
        // GET api/fruits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fruit>> Get(long id)
        {
            var fruit = await _context.Fruits.FindAsync(id);

            if (fruit == null)
            {
                return NotFound();
            }

            return fruit;
        }

        // POST api/fruits
        [HttpPost]
        public async Task<ActionResult<Fruit>> Post([FromBody] Fruit fruit)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity();
            }

            _context.Fruits.Add(fruit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = fruit.Id }, fruit);
        }

        // PUT api/fruits/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Fruit>> Put(long id, [FromBody] Fruit fruit)
        {
            
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity();
            }

            if (fruit.Id == 0)
            {
                fruit.Id = id;
            }

            if (id != fruit.Id)
            {
                return BadRequest();
            }

            var existingFruit = await _context.Fruits.FindAsync(id);

            if (existingFruit == null)
            {
                return NotFound();
            }

            existingFruit.Name = fruit.Name;
            existingFruit.Stock = fruit.Stock;

            _context.Update(existingFruit);
            await _context.SaveChangesAsync();

            return fruit;
        }

        // DELETE api/fruits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            
        }
        */
    }
}
