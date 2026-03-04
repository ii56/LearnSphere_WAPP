SELECT u.uname, i.courseid
FROM Invoice i
INNER JOIN [User] u ON i.userid = u.userid
WHERE i.courseid IN (30,31)
ORDER BY i.courseid;