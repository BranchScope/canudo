from config import DB_HOST, DB_USERNAME, DB_PASSWORD, DB_NAME
import asyncpg

#the databas class, very magical cool stuff here
class Database:
    def __init__(self):
        self.pool = None
        
    async def connect(self):
        pool = await asyncpg.create_pool(
            host=DB_HOST,
            user=DB_USERNAME,
            password=DB_PASSWORD,
            database=DB_NAME
        )
        self.pool = pool

    async def check_user(self, user_id):
        try:
            sql = "SELECT * FROM users WHERE user_id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, int(user_id))   
                if row is not None:
                    return True
                else:
                    return False
        except Exception as e:
            print(e)
            return False

    async def add_user(self, user_id, first_name, last_name, username, lang, status): #most of them are useless things, but you know, the world changes and we must prevent (no-sense)
        try:
            sql = "INSERT INTO users(user_id, first_name, last_name, username, lang, status, rank, banned) VALUES($1, $2, $3, $4, $5, $6, $7, $8) ON CONFLICT (user_id) DO UPDATE SET first_name = $2, last_name = $3, username = $4"
            async with self.pool.acquire() as con:
                await con.execute(sql, user_id, first_name, last_name, username, lang, status, 0, False)
                return True
        except Exception as e:
            print(e)
            return False

    async def get_subs(self):
        try:
            sql = "SELECT COUNT(*) FROM users"
            async with self.pool.acquire() as con:
                fetch = await con.fetch(sql)
                return fetch[0]['count']
        except Exception as e:
            print(e)
            return False

    async def get_users(self):
        try:
            sql = "SELECT user_id FROM users"
            async with self.pool.acquire() as con:
                rows = await con.fetch(sql)
                return rows
        except Exception as e:
            print(e)
            return False

    async def get_user(self, user_id):
        try:
            sql = "SELECT * FROM users WHERE user_id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, int(user_id))
                return row
        except Exception as e:
            print(e)
            return False

    async def get_user_rank(self, user_id):
        try:
            sql = "SELECT rank FROM users WHERE user_id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, user_id)
                return row['rank']
        except Exception as e:
            print(e)
            return None

    async def get_status(self, user_id):
        try:
            sql = "SELECT status FROM users WHERE user_id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, user_id)
                return row['status']
        except Exception as e:
            print(e)
            return None

    async def set_status(self, user_id, status):
        try:
            sql = "UPDATE users SET status = $2 WHERE user_id = $1"
            async with self.pool.acquire() as con:
                await con.execute(sql, user_id, status)
                return True
        except Exception as e:
            print(e)
            return False

    async def is_banned(self, user_id):
        try:
            sql = "SELECT banned FROM users WHERE user_id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, user_id)
                return row['banned']
        except Exception as e:
            return False

    async def ban_user(self, user_id):
        try:
            checku = await check_user(pool, user_id)
            if checku:
                sql = "UPDATE users SET banned = True WHERE user_id = $1"
                async with self.pool.acquire() as con:
                    await con.execute(sql, int(user_id))
                    return True
            else:
                return False
        except Exception as e:
            print(e)
            return False

    async def unban_user(self, user_id):
        try:
            checku = await check_user(pool, user_id)
            if checku:
                sql = "UPDATE users SET banned = False WHERE user_id = $1"
                async with self.pool.acquire() as con:
                    await con.execute(sql, int(user_id))
                    return True
            else:
                return False
        except Exception as e:
            print(e)
            return False

    async def get_lang(self, user_id):
        try:
            sql = "SELECT lang FROM users WHERE user_id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, user_id)
                return row['lang']
        except Exception as e:
            print(e)
            return None

    async def set_lang(self, user_id, lang):
        try:
            sql = "UPDATE users SET lang = $2 WHERE user_id = $1"
            async with self.pool.acquire() as con:
                await con.execute(sql, user_id, lang)
                return True
        except Exception as e:
            print(e)
            return False