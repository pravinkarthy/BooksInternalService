using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Books.Service.Internal.Api.Models;
using Books.Service.Internal.Api.Infrastructure;
using Books.Service.Internal.Api.Infrastructure.IdentityModels;

namespace customerapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ReadersDBContext _context;
        private JWTSetting _setting;
        private IRefreshTokenGenerator _tokengen;

        public UserController(ReadersDBContext context, IOptions<JWTSetting> setting, IRefreshTokenGenerator tokengen)
        {
            _context = context;
            _setting = setting.Value;
            _tokengen = tokengen;
        }

        // GET: api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST: api/User
        [Route("authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCredentials usercred)
        {
            var tokenresponse = new TokenResponse();

            var user = _context.TblUsers.FirstOrDefault(o => o.Userid == usercred.UserName && o.Password == usercred.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = System.Text.Encoding.UTF8.GetBytes(_setting.securitykey);
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity
                (
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Userid),
                        new Claim(ClaimTypes.Role, user.Role),
                    }
                ),
                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenhandler.CreateToken(tokendescriptor);

            tokenresponse.JWTToken = tokenhandler.WriteToken(token);
            tokenresponse.RefreshToken = _tokengen.GenerateToken(usercred.UserName);

            return Ok(tokenresponse);
        }


        // POST: api/refresh
        [Route("refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var principal = tokenhandler.ValidateToken(token.JWTToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.securitykey)),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out var securityToken);

            var _token = securityToken as JwtSecurityToken;
            if (_token != null && !_token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return Unauthorized();
            }

            var username = principal.Identity.Name;
            var _reftable = _context.TblRefreshtokens.FirstOrDefault(o => o.UserId == username && o.RefreshToken == token.RefreshToken);

            if (_reftable == null)
            {
                return Unauthorized();
            }

            return Ok(AuthenticateToken(username, principal.Claims.ToArray()));

        }

        [NonAction]
        public TokenResponse AuthenticateToken(string username, Claim[] claims)
        {
            TokenResponse tokenresponse = new();
            var tokenkey = Encoding.UTF8.GetBytes(_setting.securitykey);

            var tokenhandler = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                );

            tokenresponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenresponse.RefreshToken = _tokengen.GenerateToken(username);

            return tokenresponse;
        }

        [Route("HaveAccess")]
        [HttpGet]
        public IActionResult HaveAccess(string role, string menu)
        {
            APIResponse result = new APIResponse();
            //var username = principal.Identity.Name;
            var _result = _context.TblPermissions.Where(o => o.RoleId == role && o.MenuId == menu).FirstOrDefault();
            if (_result != null)
            {
                result.result = "pass";
            }
            return Ok(result);
        }

        [Route("GetAllRole")]
        [HttpGet]
        public IActionResult GetAllRole()
        {
            var _result = _context.TblRoles.ToList();
            // var _result = context.TblPermission.Where(o => o.RoleId == role).ToList();

            return Ok(_result);
        }

        [HttpPost("Register")]
        public APIResponse Register([FromBody] TblUser value)
        {
            string result = string.Empty;
            try
            {
                var _emp = _context.TblUsers.FirstOrDefault(o => o.Userid == value.Userid);
                if (_emp != null)
                {
                    result = string.Empty;
                }
                else
                {
                    TblUser tblUser = new TblUser()
                    {
                        Userid = value.Userid,
                        Name = value.Name,
                        Email = value.Email,
                        Role = string.Empty,
                        Password = value.Password,
                        IsActive = true
                    };
                    _context.TblUsers.Add(tblUser);
                    _context.SaveChanges();
                    result = "pass";
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return new APIResponse { keycode = string.Empty, result = result };
        }

    }

    public class APIResponse
    {
        public string keycode { get; set; }
        public string result { get; set; }
    }
}
