using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.API.Controllers
{
    
    public class AccountsController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }
        // Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExist(model.Email).Result.Value)
                return BadRequest(new ApiResponse(400, "This Email is Already Exists"));
            var User = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
            };
            var Result = await userManager.CreateAsync(User, model.Password);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
            var ReturnedObject = new UserDto()
            {
                Email = model.Email,
                DisplayName = model.DisplayName,
                Token = await tokenService.CreateTokenAsync(User,userManager)
            };
            return Ok(ReturnedObject);
        }

        // Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
          var User = await userManager.FindByEmailAsync(model.Email);
            if (User is null) return Unauthorized(new ApiResponse(401));
          var Result = await signInManager.CheckPasswordSignInAsync(User,model.Password,false);
            if (!Result.Succeeded) return Unauthorized(new ApiResponse(401));
            var ReturnedObject = new UserDto()
            {
                Email = User.Email,
                DisplayName = User.DisplayName,
                Token = await tokenService.CreateTokenAsync(User, userManager)
            };
            return Ok(ReturnedObject);
            
        }

        // Get Current User
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
           var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(Email);
            var ReturnedObject = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await tokenService.CreateTokenAsync(user, userManager)
            };
            return Ok(ReturnedObject);
        }

        // Get User Address
        [Authorize]
        [HttpGet("Address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
           var user = await userManager.FindUserWithAddressAsync(User);
            var MappedAddress = mapper.Map<Address,AddressDto>(user.Address);
            return Ok(MappedAddress);
        }

        // Update User Address
        [Authorize]
        [HttpPut("Address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto UpdatedAddress)
        {
           var user = await userManager.FindUserWithAddressAsync(User);
            // user.Address = UpdatedAddress;
            var MappedAddress = mapper.Map<AddressDto, Address>(UpdatedAddress);
            MappedAddress.Id = user.Address.Id;
            user.Address = MappedAddress;
            var Result = await userManager.UpdateAsync(user);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(UpdatedAddress);
        }

        // Check Email Exist
        [HttpGet("EmailExist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            return await userManager.FindByEmailAsync(email) is not null;
        }
    }
}
