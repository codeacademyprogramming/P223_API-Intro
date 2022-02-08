using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopApiP223.Data.DAL;
using ShopApiP223.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApiP223.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public ProductsController(ShopDbContext context)
        {
            _context = context;
        }
        
        //[Route("{id}")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Product product = _context.Products.FirstOrDefault(x=>x.Id == id);

            if (product == null) return NotFound();

            //return StatusCode(200, product);
            return Ok(product);
        }

        [Route("")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return StatusCode(200, _context.Products.Where(x=>x.DisplayStatus).ToList());
        }

        [Route("")]
        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return StatusCode(201,product);
        }

        [HttpPut("")]
        public IActionResult Update(Product product)
        {
            Product existProduct = _context.Products.FirstOrDefault(x => x.Id == product.Id);

            if (existProduct == null)
                return NotFound();

            existProduct.Name = product.Name;
            existProduct.SalePrice = product.SalePrice;
            existProduct.CostPrice = product.CostPrice;

            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Product product = _context.Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();


            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult ChangeStatus(int id,bool status)
        {
            Product product = _context.Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
                return NotFound();

            product.DisplayStatus = status;
            _context.SaveChanges();

            return NoContent();
        }
    }
}
