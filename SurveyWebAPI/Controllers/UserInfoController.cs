using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SurveyWebAPI.DataContext;
using SurveyWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace SurveyWebAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    //[AllowAnonymous]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly ManageDataContext _context;
        public UserInfoController(ManageDataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 无参GET请求
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<UserInfo>), Status200OK)]
        public async Task<ActionResult> Get()
        {
            return Ok(_context.UserInfos.ToArray());
        }
        /// <summary>
        /// 有参GET请求
        /// </summary>
        /// <param name="id">用户编号id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserInfo), Status200OK)]
        [ProducesResponseType(typeof(string), Status404NotFound)]
        public async Task<ActionResult> Get(string id)
        {
            var res = await _context.UserInfos.FindAsync(id);
            if (res != null) return Ok(res);
            else return NotFound("Cannot find key.");
        }
        /// <summary>
        /// PUT请求，新增/覆盖一条数据。
        /// </summary>
        /// <param name="value">用户JSON对象</param>
        /// <returns>是否执行成功</returns>
        [HttpPut]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(typeof(string), Status400BadRequest)]
        public async Task<ActionResult> Put([FromBody] Models.UserInfo value)
        {
            if (string.IsNullOrWhiteSpace(value.Password)) return BadRequest("Invalid password.");
            value.Role = UserInfo.GetRole(value.Role);
            value.PasswordHash = PasswordStorage.CreateHash(value.Password);
            var res = await _context.UserInfos.FindAsync(value.UserName);
            if (res != null)
            {
                _context.Entry(res).CurrentValues.SetValues(value);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                _context.UserInfos.Add(value);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

        /// <summary>
        /// Delete请求，删除一条数据
        /// </summary>
        /// <param name="id">删除数据记录的id</param>
        /// <returns>是否执行成功</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), Status204NoContent)]
        [ProducesResponseType(typeof(string), Status404NotFound)]
        public async Task<ActionResult> Delete(string id)
        {
            var res = await _context.UserInfos.FindAsync(id);
            if (res != null)
            {
                _context.UserInfos.Remove(res);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else return NotFound("Cannot find key.");
        }
    }
}
