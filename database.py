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

    async def add_user(self, user_id, first_name, last_name, username, lang): #most of them are useless parameters, but I hack my friends and I store their stuffs in CDs because my name is Sam Sepiol (cringe)
        try:
            sql = "INSERT INTO users(user_id, first_name, last_name, username, lang, rank, banned) VALUES($1, $2, $3, $4, $5, $6, $7) ON CONFLICT (user_id) DO UPDATE SET first_name = $2, last_name = $3, username = $4"
            async with self.pool.acquire() as con:
                await con.execute(sql, user_id, first_name, last_name, username, lang, 0, False)
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

    #here comes the fun, turuturu
    async def get_subjects(self):
        try:
            sql = "SELECT * FROM subjects"
            async with self.pool.acquire() as con:
                rows = await con.fetch(sql)
                return rows
        except Exception as e:
            print(e)
            return False

    async def get_subject(self, code_name):
        try:
            sql = "SELECT name FROM subjects WHERE code_name = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, code_name)
                return row['name']
        except Exception as e:
            print(e)
            return None

    async def add_subject(self, code_name, name):
        try:
            sql = "INSERT INTO subjects(code_name, name) ON CONFLICT (code_name) DO UPDATE SET name = $2"
            async with self.pool.acquire() as con:
                await con.execute(sql, code_name, name)
                return True
        except Exception as e:
            print(e)
            return False

    async def get_ad_by_id(self, id):
        try:
            sql = "SELECT * FROM ads WHERE id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, int(id))
                return row
        except Exception as e:
            print(e)
            return False

    async def create_ad(self, from_user):
        try:
            sql = "INSERT INTO ads(from_user) VALUES($1) RETURNING id"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, from_user)
                return row['id']
        except Exception as e:
            print(e)
            return False
    
    async def update_ad(self, id, whatever_it_takes, value): #(UOOOOOH) 'cause I love how it feels when I break the chains
        try:
            sql = f"UPDATE ads SET {whatever_it_takes} = $2 WHERE id = $1"
            async with self.pool.acquire() as con:
                await con.execute(sql, int(id), value)
                return True
        except Exception as e:
            print(e)
            return False

    async def delete_ad(self, id):
        try:
            sql = "DELETE FROM ads WHERE id = $1"
            async with self.pool.acquire() as con:
                row = await con.fetchrow(sql, int(id))
                return row
        except Exception as e:
            print(e)
            return False
