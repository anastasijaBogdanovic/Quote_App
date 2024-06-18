using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quotes_app.Helpers;
using quotes_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quotes_app.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly QuotesDbContext Context;
        private readonly JwtService JWTService;

        public QuotesController(QuotesDbContext context, JwtService jwtService)
        {
            Context = context;
            JWTService = jwtService;
        }

        [Route("GetQuotes")]
        [HttpGet]
        public async Task<IActionResult> GetQuotes(int page = 1)
        {
            try
            {
                const int pageSize = 5;

                var quotes = await Context.Quotes.Select(q => new
                {
                    Quote = q,
                    ID = q.Id,
                    PositiveRatingsCount = q.Ratings.Count(r => r.Value == true),
                    NegativeRatingsCount = q.Ratings.Count(r => r.Value == false),
                    TotalRatingsCount = q.Ratings.Count(),
                    sortingKey = q.Ratings.Count() > 0 ? (double)q.Ratings.Count(r => r.Value == true) / q.Ratings.Count() : 0
                }).ToListAsync();

        
                var sortedQuotes = quotes.OrderByDescending(q => q.sortingKey);

                var totalCount = sortedQuotes.Count();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                if (page < 1 || page > totalPages)
                    return BadRequest("Invalid page number.");

                var quotesForPage = sortedQuotes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(q => new
                    {
                        ID = q.ID,
                        PositiveRatingsCount = q.PositiveRatingsCount,
                        NegativeRatingsCount = q.NegativeRatingsCount,
                        Percentage = q.TotalRatingsCount > 0 ? Math.Round((double)q.PositiveRatingsCount / q.TotalRatingsCount * 100, 2) : 0,
                        Quote = q.Quote.QuoteContent,
                        Author = q.Quote.Author
                    }).ToList();

                var response = new
                {
                    Quotes = quotesForPage,
                    TotalPages = totalPages,
                    CurrentPage = page
                };

                if (!quotes.Any())
                {
                    return NotFound("No quotes found.");
                }

                return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


        [HttpPost("AddQuote")]
        public async Task<IActionResult> AddQuote([FromBody] Quote quote)
        {
            if (quote == null)
                return BadRequest("Quote object is null.");

            if (string.IsNullOrWhiteSpace(quote.QuoteContent))
                return BadRequest("Quote content is required.");

            if (string.IsNullOrWhiteSpace(quote.Author))
                return BadRequest("Author is required.");

            if (quote.Tags == null || !quote.Tags.Any())
                return BadRequest("At least one tag is required.");

            // U pocetku nema ocenu
            quote.Ratings = new List<Rating>();

            Context.Quotes.Add(quote);
            await Context.SaveChangesAsync();

            foreach (var tag in quote.Tags)
            {
                // Provera da li postoji tag sa datim imenom
                var existingTag = await Context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);
                if (existingTag == null)
                {
                    // Ako tag ne postoji, dodajemo novi tag u bazu
                    existingTag = new Tag { Name = tag.Name };
                    Context.Tags.Add(existingTag);
                }

                // Dodajemo ID citata u listu QuoteIds za tag
                existingTag.QuoteIds.Add(quote.Id);
            }

            await Context.SaveChangesAsync();

            return Ok("Good");
        }
    

        [HttpGet("GetQuotesByTags")]
        public async Task<IActionResult> GetQuotesByTags([FromQuery] List<string> tagNames, int page = 1)
        {
            try
            {
                const int pageSize = 5;

                if (tagNames == null || !tagNames.Any())
                    return BadRequest("At least one tag name must be provided.");

                // Pronalazimo tagove sa datim imenima
                var tags = await Context.Tags
                    .Where(t => tagNames.Contains(t.Name))
                    .ToListAsync();

                // Dobijamo listu ID-jeva citata za sve pronađene tagove
                var quoteIds = tags.SelectMany(t => t.QuoteIds).Distinct();

                var quotesQuery = Context.Quotes
                    .Include(q => q.Ratings)
                    .Where(q => quoteIds.Contains(q.Id));

                var quotesInfo = await quotesQuery.Select(q => new
                {
                    Quote = q,
                    ID = q.Id,
                    PositiveRatingsCount = q.Ratings.Count(r => r.Value == true),
                    NegativeRatingsCount = q.Ratings.Count(r => r.Value == false),
                    TotalRatingsCount = q.Ratings.Count(),
                    sortingKey = q.Ratings.Count() > 0 ? (double)q.Ratings.Count(r => r.Value == true) / q.Ratings.Count() : 0
                }).ToListAsync();

                var sortedQuotes = quotesInfo.OrderByDescending(q => q.sortingKey);

                var totalCount = sortedQuotes.Count();
                var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);

                if (page < 1 || page > totalPages){
                    return BadRequest("Invalid page number.");
                }

                var quotesForPage = sortedQuotes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(q => new
                    {
                        ID = q.ID,
                        PositiveRatingsCount = q.PositiveRatingsCount,
                        NegativeRatingsCount = q.NegativeRatingsCount,
                        Percentage = q.TotalRatingsCount > 0 ? Math.Round((double)q.PositiveRatingsCount / q.TotalRatingsCount * 100, 2) : 0,
                        Quote = q.Quote.QuoteContent,
                        Author = q.Quote.Author,
                        Tags = q.Quote.Tags.Select(t => t.Name).ToList()
                    }).ToList();

                var response = new
                {
                    Quotes = quotesForPage,
                    TotalPages = totalPages,
                    CurrentPage = page
                };

                if (!quotesInfo.Any())
                {
                    return NotFound("No quotes found.");
                }

                return Ok(quotesInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("RateQuote")]
        public async Task<IActionResult> RateQuote(int quoteId, bool value, int userId = 1)
        {
            // Posto u zadatku pise da nije potrebno pozivati funkcije signup i login ovaj deo sa try catch je zakomentarisan
            // ali da imamo koriscenje i tih funkcija ne bi slali userId nego token koji je u lokalnom skladistu korisnika, 
            // pa iz tokena vadili userId i proveravali da li nije isteko ili ne, tj. da li moze da izvrsi akciju

            //try
            //{
                //var jwt = Request.Cookies["jwt"];
                //var token = JwtService.Verify(jwt);
                //int userId = int.Parse(token.Issuer);z
            
                var existingRating = await Context.Ratings.FirstOrDefaultAsync(r => r.QuoteId == quoteId && r.UserId == userId);

                if (existingRating != null)
                {
                    // Korisnik je već glasao, proveri da li je ocena ista kao nova ocena
                    if (existingRating.Value == value)
                    {
                        // Korisnik je pokušao da glasa istom ocenom, obriši njegov glas iz baze
                        Context.Ratings.Remove(existingRating);
                        await Context.SaveChangesAsync();
                        return Ok("Rating removed successfully.");
                    }
                    else
                    {
                        // Korisnik je promenio svoj glas
                        existingRating.Value = value;
                        await Context.SaveChangesAsync();
                        return Ok("Rating updated successfully.");
                    }
                }
                else
                {
                    // Dodavanje novog glasa
                    var newRating = new Rating { QuoteId = quoteId, Value = value, UserId = userId };
                    Context.Ratings.Add(newRating);
                    await Context.SaveChangesAsync();
                    return Ok("Rating added successfully.");
                }
            //catch(Exception e)
            //{
            //    return BadRequest(error: new {message= "Invalid Credentials"});
            //}
        }

    }