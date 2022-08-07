using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaAPICore.Controllers
{
    [Route("api/[controller]")]
    //[ApiConventionType(typeof(DefaultApiConventions))]
    public class ValuesController : MainController // maincontroller é uma classe apartada criada para customizar algumas coisas e diminuir a quantidade de codigo na controller
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> ObterTodos()
        {
            var valores = new string[] { "value1", "value2" };

            if (valores.Length < 5000)
                return BadRequest();

            return valores; // aqui não é obrigatorio retornar um actionresult (Ok, forbid, badrequest, notfound etc) pois a assinatura ja diz que será retornado um IEnumerable
        }

        [HttpGet]
        public ActionResult ObterResultado()
        {
            var valores = new string[] { "value1", "value2" };

            if (valores.Length < 5000)
                return CustomResponse();

            return CustomResponse(valores); // aqui é obrigatorio retornar um actionresult pois a assinatura do método diz apenas que só será retornado um actionResult
        }

        [HttpGet("obter-valores")]
        public IEnumerable<string> ObterValores()
        {
            var valores = new string[] { "value1", "value2" };

            if (valores.Length < 5000)
                return null; // não tem como retornar BadRequest()

            return valores;
        }

        // GET api/values/obter-por-id/5
        [HttpGet("obter-por-id/{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        // [ProducesResponseType(typeof(Produto), StatusCodes.Status201Created)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        //public void Post([FromBody] Product value)
        public ActionResult Post(Product product) 
        {
            // quando está explicito o tipo complexo - no caso Product - a controller já entende que é um FromBody, não precisa colocar
            // no caso, só vai precisar colocar quando vier de outra forma: FromHeader, FromForm, FromRoute etc

            if (product.Id == 0) return BadRequest();

            // add no banco 

            //return Ok(product);
            //return CreatedAtAction("Post", product);
            return CreatedAtAction(nameof(Post), product);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public ActionResult Put([FromRoute] int id, [FromForm] Product product)
        {
            // FromRoute deixa explícito que o id virá na rota, porem não é necessário
            // FromForm indica que o objeto virá no formulário da solicitação

            if (!ModelState.IsValid) return BadRequest();

            if (id != product.Id) return NotFound();

            return NoContent(); // o put não retorna nada. a convenção sugere que seja usado um nocontent result
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = ObterErros()
            }); ;
        }
        public bool OperacaoValida()
        {
            // as minhas validações
            return true;
        }

        protected string ObterErros()
        {
            return "";
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
