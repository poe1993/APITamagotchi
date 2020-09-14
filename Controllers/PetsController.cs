using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITamagotchi.Models;

namespace APITamagotchi.Controllers
{
    // All of these routes will be at the base URL:     /api/Pets
    // That is what "api/[controller]" means below. It uses the name of the controller
    // in this case PetsController to determine the URL
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        // This is the variable you use to have access to your database
        private readonly DatabaseContext _context;

        // Constructor that recives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public PetsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Pets
        //
        // Returns a list of all your Pets
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pets>>> GetPets()
        {
            // Uses the database context in `_context` to request all of the Pets, sort
            // them by row id and return them as a JSON array.
            return await _context.Pets.OrderBy(row => row.Id).ToListAsync();
        }

        // GET: api/Pets/5
        //
        // Fetches and returns a specific pets by finding it by id. The id is specified in the
        // URL. In the sample URL above it is the `5`.  The "{id}" in the [HttpGet("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Pets>> GetPets(int id)
        {
            // Find the pets in the database using `FindAsync` to look it up by id
            var pets = await _context.Pets.FindAsync(id);

            // If we didn't find anything, we receive a `null` in return
            if (pets == null)
            {
                // Return a `404` response to the client indicating we could not find a pets with this id
                return NotFound();
            }

            //  Return the pets as a JSON object.
            return pets;
        }

        // PUT: api/Pets/5
        //
        // Update an individual pets with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpPut("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        // In addition the `body` of the request is parsed and then made available to us as a Pets
        // variable named pets. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Pets POCO class. This represents the
        // new values for the record.
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPets(int id, Pets pets)
        {
            // If the ID in the URL does not match the ID in the supplied request body, return a bad request
            if (id != pets.Id)
            {
                return BadRequest();
            }

            // Tell the database to consider everything in pets to be _updated_ values. When
            // the save happens the database will _replace_ the values in the database with the ones from pets
            _context.Entry(pets).State = EntityState.Modified;

            try
            {
                // Try to save these changes.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ooops, looks like there was an error, so check to see if the record we were
                // updating no longer exists.
                if (!PetsExists(id))
                {
                    // If the record we tried to update was already deleted by someone else,
                    // return a `404` not found
                    return NotFound();
                }
                else
                {
                    // Otherwise throw the error back, which will cause the request to fail
                    // and generate an error to the client.
                    throw;
                }
            }

            // return NoContent to indicate the update was done. Alternatively you can use the
            // following to send back a copy of the updated data.
            //
            // return Ok(pets)
            //
            return NoContent();
        }

        // POST: api/Pets
        //
        // Creates a new pets in the database.
        //
        // The `body` of the request is parsed and then made available to us as a Pets
        // variable named pets. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Pets POCO class. This represents the
        // new values for the record.
        //
        [HttpPost]
        public async Task<ActionResult<Pets>> PostPets(Pets pets)
        {
            // Indicate to the database context we want to add this new record
            _context.Pets.Add(pets);
            await _context.SaveChangesAsync();

            // Return a response that indicates the object was created (status code `201`) and some additional
            // headers with details of the newly created object.
            return CreatedAtAction("GetPets", new { id = pets.Id }, pets);
        }

        // DELETE: api/Pets/5
        //
        // Deletes an individual pets with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpDelete("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePets(int id)
        {
            // Find this pets by looking for the specific id
            var pets = await _context.Pets.FindAsync(id);
            if (pets == null)
            {
                // There wasn't a pets with that id so return a `404` not found
                return NotFound();
            }

            // Tell the database we want to remove this record
            _context.Pets.Remove(pets);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            // return NoContent to indicate the update was done. Alternatively you can use the
            // following to send back a copy of the deleted data.
            //
            // return Ok(pets)
            //
            return NoContent();
        }
        [HttpPost("{Id}/Playtimes")]
        public async Task<ActionResult<PlayTime>> Play(int id, PlayTime playtime)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            pet.HappinessLevel += 5;
            pet.HungerLevel += 3;
            playtime.When = DateTime.Now;
            playtime.PetId = pet.Id;
            _context.PlayTime.Add(playtime);
            await _context.SaveChangesAsync();
            return Ok(playtime);
        }
        [HttpPost("{Id}/Feedings")]
        public async Task<ActionResult<Feedings>> Feed(int id, Feedings feeding)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            pet.HappinessLevel += 3;
            pet.HungerLevel -= 5;
            if (pet.HungerLevel < 0)
            {
                pet.HungerLevel = 0;
            }
            feeding.When = DateTime.Now;
            feeding.PetId = pet.Id;
            _context.Feedings.Add(feeding);
            await _context.SaveChangesAsync();
            return Ok(feeding);
        }
        [HttpPost("{Id}/Scoldings")]
        public async Task<ActionResult<Scoldings>> Scold(int id, Scoldings scolding)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            pet.HappinessLevel -= 5;
            if (pet.HappinessLevel < 0)
            {
                pet.HappinessLevel = 0;
            }
            scolding.When = DateTime.Now;
            scolding.PetId = pet.Id;
            _context.Scoldings.Add(scolding);
            await _context.SaveChangesAsync();
            return Ok(scolding);
        }

        // Private helper method that looks up an existing pets by the supplied id
        private bool PetsExists(int id)
        {
            return _context.Pets.Any(pets => pets.Id == id);
        }
    }
}
